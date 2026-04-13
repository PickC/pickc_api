namespace PickC.Modules.Identity.Domain.Entities;

public class DriverActivity
{
    public Guid TokenNo { get; set; }
    public string DriverId { get; set; } = string.Empty;
    public string VehicleNo { get; set; } = string.Empty;
    public bool IsLogIn { get; set; }
    public DateTime? LoginDate { get; set; }
    public DateTime? LogoutDate { get; set; }
    public bool IsOnDuty { get; set; }
    public DateTime? DutyOnDate { get; set; }
    public DateTime? DutyOffDate { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public decimal? CurrentLat { get; set; }
    public decimal? CurrentLong { get; set; }
    public decimal? LogOutLat { get; set; }
    public decimal? LogOutLong { get; set; }
    public bool IsBusy { get; set; }
}
