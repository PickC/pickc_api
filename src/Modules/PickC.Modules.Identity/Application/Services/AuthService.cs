using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using PickC.Modules.Identity.Application.DTOs;
using PickC.Modules.Identity.Domain.Entities;
using PickC.Modules.Identity.Domain.Interfaces;
using PickC.Modules.Identity.Infrastructure.Data;
using PickC.SharedKernel.Helpers;

namespace PickC.Modules.Identity.Application.Services;

public class AuthService : IAuthService
{
    private readonly IdentityDbContext _context;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly JwtSettings _jwtSettings;

    public AuthService(
        IdentityDbContext context,
        IJwtTokenService jwtTokenService,
        IOptions<JwtSettings> jwtSettings)
    {
        _context = context;
        _jwtTokenService = jwtTokenService;
        _jwtSettings = jwtSettings.Value;
    }

    public async Task<LoginResponseDto?> LoginCustomerAsync(
        string mobileNo, string password, decimal? latitude, decimal? longitude)
    {
        var customer = await _context.CustomerCredentials
            .FirstOrDefaultAsync(c => c.MobileNo == mobileNo && c.Password == password);

        if (customer is null)
            return null;

        // Upsert Operation.CustomerLogin
        var session = await _context.CustomerLogins
            .FirstOrDefaultAsync(c => c.MobileNo == mobileNo);

        if (session is null)
        {
            _context.CustomerLogins.Add(new CustomerLogin
            {
                TokenNo = Guid.NewGuid(),
                MobileNo = mobileNo,
                Status = true,
                LoginTime = IstClock.Now,
                LogoutTime = null,
                CurrentLat = latitude,
                CurrentLong = longitude
            });
        }
        else
        {
            session.Status = true;
            session.LoginTime = IstClock.Now;
            session.LogoutTime = null;
            session.CurrentLat = latitude;
            session.CurrentLong = longitude;
        }

        var accessToken = _jwtTokenService.GenerateAccessToken(
            customer.MobileNo, "CUSTOMER", customer.MobileNo);
        var refreshToken = _jwtTokenService.GenerateRefreshToken();

        await SaveRefreshTokenAsync(customer.MobileNo, "CUSTOMER", refreshToken);

        return new LoginResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresIn = _jwtSettings.AccessTokenExpirationMinutes * 60,
            UserType = "CUSTOMER",
            UserId = customer.MobileNo
        };
    }

    public async Task<LoginResponseDto?> LoginDriverAsync(
        string driverId, string password, decimal? latitude, decimal? longitude)
    {
        var driver = await _context.DriverCredentials
            .FirstOrDefaultAsync(d => d.DriverId == driverId && d.Password == password);

        if (driver is null || !driver.Status)
            return null;

        // Upsert Operation.DriverActivity
        var activity = await _context.DriverActivities
            .FirstOrDefaultAsync(d => d.DriverId == driverId);

        if (activity is null)
        {
            _context.DriverActivities.Add(new DriverActivity
            {
                TokenNo = Guid.NewGuid(),
                DriverId = driverId,
                VehicleNo = driver.VehicleNo,
                IsLogIn = true,
                LoginDate = IstClock.Now,
                LogoutDate = null,
                Latitude = latitude,
                Longitude = longitude,
                CurrentLat = latitude,
                CurrentLong = longitude,
                IsOnDuty = true,
                DutyOnDate = IstClock.Now,
                IsBusy = false
            });
        }
        else
        {
            activity.VehicleNo = driver.VehicleNo;
            activity.IsLogIn = true;
            activity.LoginDate = IstClock.Now;
            activity.LogoutDate = null;
            activity.Latitude = latitude;
            activity.Longitude = longitude;
            activity.CurrentLat = latitude;
            activity.CurrentLong = longitude;
            activity.IsOnDuty = true;
            activity.DutyOnDate = IstClock.Now;
            activity.IsBusy = false;
        }

        var accessToken = _jwtTokenService.GenerateAccessToken(
            driver.DriverId, "DRIVER", driver.MobileNo);
        var refreshToken = _jwtTokenService.GenerateRefreshToken();

        await SaveRefreshTokenAsync(driver.DriverId, "DRIVER", refreshToken);

        return new LoginResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresIn = _jwtSettings.AccessTokenExpirationMinutes * 60,
            UserType = "DRIVER",
            UserId = driver.DriverId
        };
    }

    public async Task<LoginResponseDto?> RefreshTokenAsync(string accessToken, string refreshToken)
    {
        var principal = _jwtTokenService.ValidateToken(accessToken);
        if (principal is null)
            return null;

        var userId = principal.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                     ?? principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var userType = principal.FindFirst("userType")?.Value;
        var mobileNo = principal.FindFirst("mobileNo")?.Value;

        if (userId is null || userType is null)
            return null;

        var storedToken = await _context.RefreshTokens
            .FirstOrDefaultAsync(r =>
                r.UserId == userId &&
                r.Token == refreshToken &&
                !r.IsRevoked &&
                r.ExpiresAt > DateTime.UtcNow);

        if (storedToken is null)
            return null;

        storedToken.IsRevoked = true;
        storedToken.RevokedAt = DateTime.UtcNow;

        var newAccessToken = _jwtTokenService.GenerateAccessToken(userId, userType, mobileNo ?? string.Empty);
        var newRefreshToken = _jwtTokenService.GenerateRefreshToken();

        await SaveRefreshTokenAsync(userId, userType, newRefreshToken);

        return new LoginResponseDto
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken,
            ExpiresIn = _jwtSettings.AccessTokenExpirationMinutes * 60,
            UserType = userType,
            UserId = userId
        };
    }

    public async Task LogoutAsync(string userId, string userType, decimal? latitude = null, decimal? longitude = null)
    {
        // Revoke all refresh tokens
        var tokens = await _context.RefreshTokens
            .Where(r => r.UserId == userId && !r.IsRevoked)
            .ToListAsync();

        foreach (var token in tokens)
        {
            token.IsRevoked = true;
            token.RevokedAt = DateTime.UtcNow;
        }

        // Update activity table
        if (userType == "DRIVER")
        {
            var activity = await _context.DriverActivities
                .FirstOrDefaultAsync(d => d.DriverId == userId);

            if (activity is not null)
            {
                activity.IsLogIn = false;
                activity.LogoutDate = IstClock.Now;
                activity.IsOnDuty = false;
                activity.DutyOffDate = IstClock.Now;
                activity.IsBusy = false;
                activity.LogOutLat = latitude;
                activity.LogOutLong = longitude;
                activity.CurrentLat = latitude;
                activity.CurrentLong = longitude;
            }
        }
        else if (userType == "CUSTOMER")
        {
            var session = await _context.CustomerLogins
                .FirstOrDefaultAsync(c => c.MobileNo == userId);

            if (session is not null)
            {
                session.Status = false;
                session.LogoutTime = IstClock.Now;
            }
        }

        await _context.SaveChangesAsync();
    }

    public async Task UpdateDutyStatusAsync(string driverId, bool isOnDuty)
    {
        var activity = await _context.DriverActivities
            .FirstOrDefaultAsync(d => d.DriverId == driverId && d.IsLogIn);

        if (activity is not null)
        {
            activity.IsOnDuty = isOnDuty;
            if (isOnDuty)
                activity.DutyOnDate = IstClock.Now;
            else
                activity.DutyOffDate = IstClock.Now;

            await _context.SaveChangesAsync();
        }
    }

    private async Task SaveRefreshTokenAsync(string userId, string userType, string token)
    {
        var refreshToken = new RefreshToken
        {
            Id = Guid.NewGuid().ToString(),
            UserId = userId,
            UserType = userType,
            Token = token,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDays),
            IsRevoked = false
        };

        _context.RefreshTokens.Add(refreshToken);
        await _context.SaveChangesAsync();
    }
}
