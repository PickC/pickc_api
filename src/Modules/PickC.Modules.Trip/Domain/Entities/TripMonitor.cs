namespace PickC.Modules.Trip.Domain.Entities;

public class TripMonitor
{
    public string DriverID { get; set; } = string.Empty;
    public string TripID { get; set; } = string.Empty;
    public string VehicleNo { get; set; } = string.Empty;
    public DateTime RefreshDate { get; set; }
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
    public short TripType { get; set; }
    public decimal? Bearing { get; set; }
    public decimal? SpeedKmh { get; set; }
}
