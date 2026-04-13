namespace PickC.Modules.Reports.Application.DTOs;

public class DailyPaymentReportDto
{
    public string InvoiceNo { get; set; } = string.Empty;
    public string BookingNo { get; set; } = string.Empty;
    public string TripID { get; set; } = string.Empty;
    public DateTime InvoiceDate { get; set; }
    public string CustomerMobile { get; set; } = string.Empty;
    public string DriverID { get; set; } = string.Empty;
    public decimal TripAmount { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal TipAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public short PaymentType { get; set; }
    public decimal PaidAmount { get; set; }
    public bool IsPaid { get; set; }
    public DateTime? PaidDate { get; set; }
}

public class DailyPaymentReportSummary
{
    public List<DailyPaymentReportDto> Payments { get; set; } = new();
    public decimal TotalTripAmount { get; set; }
    public decimal TotalTaxAmount { get; set; }
    public decimal TotalTipAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal TotalPaidAmount { get; set; }
}
