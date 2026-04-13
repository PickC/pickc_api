namespace PickC.Modules.Reports.Application.DTOs;

public class DailyTripReportDto
{
    public string TripID { get; set; } = string.Empty;
    public DateTime TripDate { get; set; }
    public string CustomerMobile { get; set; } = string.Empty;
    public string DriverID { get; set; } = string.Empty;
    public string VehicleNo { get; set; } = string.Empty;
    public string? LocationFrom { get; set; }
    public string? LocationTo { get; set; }
    public decimal DistanceKm { get; set; }
    public decimal TripMinutes { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public string? BookingNo { get; set; }
}
