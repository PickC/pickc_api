namespace PickC.Modules.Master.Domain.Entities;

public class Operator
{
    public string OperatorID { get; set; } = string.Empty;
    public string OperatorName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string FatherName { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public string PlaceOfBirth { get; set; } = string.Empty;
    public short Gender { get; set; }
    public short MaritialStatus { get; set; }
    public string MobileNo { get; set; } = string.Empty;
    public string? PhoneNo { get; set; }
    public string? PANNo { get; set; }
    public string? AadharCardNo { get; set; }
    public string Nationality { get; set; } = string.Empty;
    public bool Status { get; set; }
    public bool? IsVerified { get; set; }
    public string? VerifiedBy { get; set; }
    public DateTime? VerifiedOn { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? CreatedOn { get; set; }
    public string? ModifiedBy { get; set; }
    public DateTime? ModifiedOn { get; set; }

    // Navigation
    public ICollection<Driver> Drivers { get; set; } = new List<Driver>();
    public BankDetails? BankAccount { get; set; }
}
