namespace PickC.Modules.Reports.Application.DTOs;

public class SummaryReportDto
{
    public int TotalBookings { get; set; }
    public int ConfirmedBookings { get; set; }
    public int CompletedBookings { get; set; }
    public int CancelledBookings { get; set; }
    public int TotalTrips { get; set; }
    public decimal TotalDistanceKm { get; set; }
    public decimal TotalRevenue { get; set; }
    public decimal TotalTaxAmount { get; set; }
    public decimal TotalTipAmount { get; set; }
    public decimal TotalPaidAmount { get; set; }
    public decimal OutstandingAmount { get; set; }
    public int CashPaymentCount { get; set; }
    public decimal CashPaymentAmount { get; set; }
    public int CreditPaymentCount { get; set; }
    public decimal CreditPaymentAmount { get; set; }
    public int OnlinePaymentCount { get; set; }
    public decimal OnlinePaymentAmount { get; set; }
    public int ActiveDrivers { get; set; }
    public List<TopDriverDto> TopDrivers { get; set; } = new();
}

public class TopDriverDto
{
    public string DriverID { get; set; } = string.Empty;
    public string DriverName { get; set; } = string.Empty;
    public int TotalTrips { get; set; }
    public decimal TotalEarnings { get; set; }
}
