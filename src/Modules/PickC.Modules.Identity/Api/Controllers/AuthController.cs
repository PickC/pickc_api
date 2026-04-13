using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PickC.Modules.Identity.Application.DTOs;
using PickC.Modules.Identity.Domain.Interfaces;

namespace PickC.Modules.Identity.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("customer/login")]
    [AllowAnonymous]
    public async Task<IActionResult> CustomerLogin([FromBody] CustomerLoginRequest request)
    {
        var result = await _authService.LoginCustomerAsync(
            request.MobileNo, request.Password,
            request.Latitude, request.Longitude);

        if (result is null)
            return Unauthorized(new { message = "Invalid credentials" });

        return Ok(result);
    }

    [HttpPost("driver/login")]
    [AllowAnonymous]
    public async Task<IActionResult> DriverLogin([FromBody] DriverLoginRequest request)
    {
        var result = await _authService.LoginDriverAsync(
            request.DriverId, request.Password,
            request.Latitude, request.Longitude);

        if (result is null)
            return Unauthorized(new { message = "Invalid credentials" });

        return Ok(result);
    }

    [HttpPost("refresh")]
    [AllowAnonymous]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        var result = await _authService.RefreshTokenAsync(
            request.AccessToken, request.RefreshToken);

        if (result is null)
            return Unauthorized(new { message = "Invalid or expired refresh token" });

        return Ok(result);
    }

    [HttpPost("logout")]
    [AllowAnonymous]
    public async Task<IActionResult> Logout([FromBody] LogoutRequest? request = null)
    {
        // Prefer JWT claims; fall back to body (handles expired-token & dev-bypass scenarios)
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                     ?? request?.DriverId;
        var userType = User.FindFirst("userType")?.Value
                       ?? request?.UserType
                       ?? "DRIVER";

        if (userId is not null)
            await _authService.LogoutAsync(userId, userType,
                request?.Latitude, request?.Longitude);

        return Ok(new { message = "Logged out successfully" });
    }

    [HttpPut("driver/duty-status")]
    [Authorize]
    public async Task<IActionResult> UpdateDutyStatus([FromBody] DutyStatusRequest request)
    {
        var driverId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (driverId is null)
            return Unauthorized(new { message = "Unauthorized" });

        await _authService.UpdateDutyStatusAsync(driverId, request.IsOnDuty);
        var msg = request.IsOnDuty ? "Driver is now on duty" : "Driver is now off duty";
        return Ok(new { message = msg, isOnDuty = request.IsOnDuty });
    }
}
