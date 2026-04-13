namespace PickC.Modules.Identity.Domain.Entities;

// Maps to Operation.CustomerLogin — session tracking table only (not credentials)
public class CustomerLogin
{
    public Guid TokenNo { get; set; }
    public string MobileNo { get; set; } = string.Empty;
    public bool Status { get; set; }
    public DateTime? LoginTime { get; set; }
    public DateTime? LogoutTime { get; set; }
    public decimal? CurrentLat { get; set; }
    public decimal? CurrentLong { get; set; }
}
