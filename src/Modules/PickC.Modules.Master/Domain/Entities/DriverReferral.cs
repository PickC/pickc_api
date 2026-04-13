namespace PickC.Modules.Master.Domain.Entities;

public class DriverReferral
{
    public int ReferralId { get; set; }
    public string DriverID { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Mobile { get; set; } = string.Empty;
    public string? EmailID { get; set; }
    public bool? IsReferralRegistered { get; set; }
    public bool? IsEligible { get; set; }
    public decimal? ReferalAmount { get; set; }
    public decimal? ReferalAmountPaid { get; set; }
    public bool? Status { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? CreatedOn { get; set; }
    public string? Modifiedby { get; set; }
    public DateTime? ModifiedOn { get; set; }
}
