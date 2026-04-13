namespace PickC.Modules.Identity.Domain.Entities;

public class DriverCredential
{
    public string DriverId { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string MobileNo { get; set; } = string.Empty;
    public string VehicleNo { get; set; } = string.Empty;
    public bool Status { get; set; }
}
