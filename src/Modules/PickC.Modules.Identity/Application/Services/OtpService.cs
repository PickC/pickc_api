using Microsoft.Extensions.Logging;
using PickC.Modules.Master.Domain.Interfaces;
using PickC.SharedKernel.Helpers;

namespace PickC.Modules.Identity.Application.Services;

public interface IOtpService
{
    string GenerateOtp(int length = 6);
    Task<bool> SendOtpAsync(string mobileNo, string otp, string userType);
    Task<bool> VerifyOtpAsync(string mobileNo, string otp, string userType);
    bool ValidateOtp(string storedOtp, DateTime? expiry, string providedOtp);
}

public class OtpService : IOtpService
{
    private readonly ICustomerRepository _customerRepository;
    private readonly ILogger<OtpService> _logger;

    public OtpService(ICustomerRepository customerRepository, ILogger<OtpService> logger)
    {
        _customerRepository = customerRepository;
        _logger = logger;
    }

    public string GenerateOtp(int length = 6)
    {
        var random = new Random();
        var min = (int)Math.Pow(10, length - 1);
        var max = (int)Math.Pow(10, length) - 1;
        return random.Next(min, max).ToString();
    }

    public async Task<bool> SendOtpAsync(string mobileNo, string otp, string userType)
    {
        _logger.LogInformation("OTP {Otp} sent to {MobileNo} ({UserType})", otp, mobileNo, userType);

        if (userType.Equals("CUSTOMER", StringComparison.OrdinalIgnoreCase))
            return await _customerRepository.SaveOtpAsync(mobileNo, otp, IstClock.Now);

        // TODO: wire DRIVER OTP storage
        return true;
    }

    public async Task<bool> VerifyOtpAsync(string mobileNo, string otp, string userType)
    {
        if (userType.Equals("CUSTOMER", StringComparison.OrdinalIgnoreCase))
            return await _customerRepository.VerifyOtpAsync(mobileNo, otp, IstClock.Now);

        // TODO: wire DRIVER OTP verification
        return false;
    }

    public bool ValidateOtp(string storedOtp, DateTime? expiry, string providedOtp)
    {
        if (string.IsNullOrEmpty(storedOtp) || !expiry.HasValue)
            return false;

        if (DateTime.UtcNow > expiry.Value)
            return false;

        return storedOtp == providedOtp;
    }
}
