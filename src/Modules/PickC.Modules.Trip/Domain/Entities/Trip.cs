namespace PickC.Modules.Trip.Domain.Entities;

public class Trip
{
    public string TripID { get; set; } = string.Empty;
    public DateTime TripDate { get; set; }
    public string CustomerMobile { get; set; } = string.Empty;
    public string DriverID { get; set; } = string.Empty;
    public string VehicleNo { get; set; } = string.Empty;
    public short VehicleType { get; set; }
    public short VehicleGroup { get; set; }
    public string? LocationFrom { get; set; }
    public string? LocationTo { get; set; }
    public decimal Distance { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public decimal TripMinutes { get; set; }
    public decimal WaitingMinutes { get; set; }
    public string TotalWeight { get; set; } = string.Empty;
    public string? CargoDescription { get; set; }
    public string? Remarks { get; set; }
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
    public decimal? TripEndLat { get; set; }
    public decimal? TripEndLong { get; set; }
    public decimal? DistanceTravelled { get; set; }
    public string? BookingNo { get; set; }
}
