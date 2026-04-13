using PickC.Modules.Identity.Application.DTOs;

namespace PickC.Modules.Identity.Domain.Interfaces;

public interface IAuthService
{
    Task<LoginResponseDto?> LoginCustomerAsync(string mobileNo, string password, decimal? latitude, decimal? longitude);
    Task<LoginResponseDto?> LoginDriverAsync(string driverId, string password, decimal? latitude, decimal? longitude);
    Task<LoginResponseDto?> RefreshTokenAsync(string accessToken, string refreshToken);
    Task LogoutAsync(string userId, string userType, decimal? latitude = null, decimal? longitude = null);
    Task UpdateDutyStatusAsync(string driverId, bool isOnDuty);
}
