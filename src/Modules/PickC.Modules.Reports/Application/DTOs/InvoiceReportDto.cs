namespace PickC.Modules.Reports.Application.DTOs;

public class InvoiceReportDto
{
    public string InvoiceNo { get; set; } = string.Empty;
    public string TripID { get; set; } = string.Empty;
    public DateTime InvoiceDate { get; set; }
    public string BookingNo { get; set; } = string.Empty;
    // Customer
    public string CustomerMobile { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerEmail { get; set; } = string.Empty;
    // Driver
    public string DriverID { get; set; } = string.Empty;
    public string DriverName { get; set; } = string.Empty;
    public string VehicleNo { get; set; } = string.Empty;
    // Trip
    public string? LocationFrom { get; set; }
    public string? LocationTo { get; set; }
    public decimal DistanceKm { get; set; }
    public decimal TripMinutes { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    // Fare
    public decimal TripAmount { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal TipAmount { get; set; }
    public decimal TotalAmount { get; set; }
    // Payment
    public short PaymentType { get; set; }
    public decimal PaidAmount { get; set; }
    public bool IsPaid { get; set; }
    public DateTime? PaidDate { get; set; }
}
