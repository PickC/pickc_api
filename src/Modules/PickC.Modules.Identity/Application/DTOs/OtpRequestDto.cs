namespace PickC.Modules.Identity.Application.DTOs;

public class OtpSendRequest
{
    public string MobileNo { get; set; } = string.Empty;
    public string UserType { get; set; } = string.Empty; // CUSTOMER or DRIVER
}

public class OtpVerifyRequest
{
    public string MobileNo { get; set; } = string.Empty;
    public string Otp { get; set; } = string.Empty;
    public string UserType { get; set; } = string.Empty;
}
