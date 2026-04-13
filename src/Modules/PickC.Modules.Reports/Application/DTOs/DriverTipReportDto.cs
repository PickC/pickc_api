namespace PickC.Modules.Reports.Application.DTOs;

public class DriverTipReportDto
{
    public DateTime Date { get; set; }
    public string InvoiceNo { get; set; } = string.Empty;
    public string TripID { get; set; } = string.Empty;
    public string BookingNo { get; set; } = string.Empty;
    public string CustomerMobile { get; set; } = string.Empty;
    public string DriverID { get; set; } = string.Empty;
    public string DriverName { get; set; } = string.Empty;
    public decimal TipAmount { get; set; }
}

public class DriverTipReportSummary
{
    public List<DriverTipReportDto> Tips { get; set; } = new();
    public decimal TotalTips { get; set; }
}
