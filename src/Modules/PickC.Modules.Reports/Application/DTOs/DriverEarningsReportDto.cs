namespace PickC.Modules.Reports.Application.DTOs;

public class DriverEarningsReportDto
{
    public string DriverID { get; set; } = string.Empty;
    public string DriverName { get; set; } = string.Empty;
    public string MobileNo { get; set; } = string.Empty;
    public int TotalTrips { get; set; }
    public decimal TotalDistanceKm { get; set; }
    public decimal TripAmount { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal TipAmount { get; set; }
    public decimal TotalEarnings { get; set; }
    public decimal PaidAmount { get; set; }
    public decimal Outstanding { get; set; }
}
