using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PickC.Modules.Identity.Application.DTOs;
using PickC.Modules.Identity.Application.Services;

namespace PickC.Modules.Identity.Api.Controllers;

[ApiController]
[Route("api/auth/otp")]
public class OtpController : ControllerBase
{
    private readonly IOtpService _otpService;

    public OtpController(IOtpService otpService)
    {
        _otpService = otpService;
    }

    [HttpPost("send")]
    [AllowAnonymous]
    public async Task<IActionResult> SendOtp([FromBody] OtpSendRequest request)
    {
        var otp = _otpService.GenerateOtp();
        var sent = await _otpService.SendOtpAsync(request.MobileNo, otp, request.UserType);

        if (!sent)
            return BadRequest(new { message = "Failed to send OTP" });

        return Ok(new { message = "OTP sent successfully" });
    }

    [HttpPost("verify")]
    [AllowAnonymous]
    public async Task<IActionResult> VerifyOtp([FromBody] OtpVerifyRequest request)
    {
        var verified = await _otpService.VerifyOtpAsync(request.MobileNo, request.Otp, request.UserType);

        if (!verified)
            return BadRequest(new { message = "Invalid or expired OTP" });

        return Ok(new { message = "OTP verified successfully", verified = true });
    }
}
