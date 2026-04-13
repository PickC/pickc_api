namespace PickC.Modules.Reports.Application.DTOs;

public class DailyBookingReportDto
{
    public string BookingNo { get; set; } = string.Empty;
    public DateTime BookingDate { get; set; }
    public string CustomerID { get; set; } = string.Empty;
    public string DriverID { get; set; } = string.Empty;
    public short VehicleType { get; set; }
    public string LocationFrom { get; set; } = string.Empty;
    public string LocationTo { get; set; } = string.Empty;
    public string CargoType { get; set; } = string.Empty;
    public int Status { get; set; }
    public bool IsCancel { get; set; }
    public bool IsComplete { get; set; }
}

public class DailyBookingReportSummary
{
    public List<DailyBookingReportDto> Bookings { get; set; } = new();
    public int Total { get; set; }
    public int Confirmed { get; set; }
    public int Completed { get; set; }
    public int Cancelled { get; set; }
}
