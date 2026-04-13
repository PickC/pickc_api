namespace PickC.Modules.Master.Domain.Entities;

public class Customer
{
    public string MobileNo { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string EmailID { get; set; } = string.Empty;
    public DateTime CreatedOn { get; set; }
    public string DeviceID { get; set; } = string.Empty;
    public string? OTP { get; set; }
    public bool? IsOTPVerified { get; set; }
    public DateTime? OTPSendDate { get; set; }
    public DateTime? OTPVerifiedDate { get; set; }

    // Navigation
    public ICollection<Address> Addresses { get; set; } = new List<Address>();
}
