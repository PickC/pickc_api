namespace PickC.Modules.Master.Domain.Entities;

public class Driver
{
    public string DriverID { get; set; } = string.Empty;
    public string DriverName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string VehicleNo { get; set; } = string.Empty;
    public string FatherName { get; set; } = string.Empty;
    public DateTime? DateOfBirth { get; set; }
    public string PlaceOfBirth { get; set; } = string.Empty;
    public short Gender { get; set; }
    public short MaritialStatus { get; set; }
    public string MobileNo { get; set; } = string.Empty;
    public string? PhoneNo { get; set; }
    public string? PANNo { get; set; }
    public string? AadharCardNo { get; set; }
    public string LicenseNo { get; set; } = string.Empty;
    public bool Status { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime CreatedOn { get; set; }
    public string ModifiedBy { get; set; } = string.Empty;
    public DateTime ModifiedOn { get; set; }
    public bool IsVerified { get; set; }
    public string VerifiedBy { get; set; } = string.Empty;
    public DateTime? VerifiedOn { get; set; }
    public string DeviceID { get; set; } = string.Empty;
    public string? Nationality { get; set; }
    public string? OperatorID { get; set; }
    public string? MobileMake { get; set; }
    public string? ModelNo { get; set; }
    public DateTime? DateofIssue { get; set; }
    public DateTime? DateofReturn { get; set; }
    public string? DeviceRemarks { get; set; }
    public string? VehicleRCNo { get; set; }

    // Navigation
    public ICollection<Address> Addresses { get; set; } = new List<Address>();
    public ICollection<DriverAttachment> Attachments { get; set; } = new List<DriverAttachment>();
    public DriverBankDetails? BankDetails { get; set; }
    public ICollection<DriverRating> Ratings { get; set; } = new List<DriverRating>();
    public ICollection<DriverReferral> Referrals { get; set; } = new List<DriverReferral>();
    public Operator? Operator { get; set; }
}
