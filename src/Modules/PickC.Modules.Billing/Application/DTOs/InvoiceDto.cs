namespace PickC.Modules.Billing.Application.DTOs;

public class InvoiceDto
{
    public string InvoiceNo { get; set; } = string.Empty;
    public string TripID { get; set; } = string.Empty;
    public DateTime InvoiceDate { get; set; }
    public decimal TripAmount { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal TipAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public short PaymentType { get; set; }
    public decimal PaidAmount { get; set; }
    public DateTime CreatedOn { get; set; }
    public bool IsMailSent { get; set; }
    public string BookingNo { get; set; } = string.Empty;
    public bool IsPaid { get; set; }
    public DateTime? PaidDate { get; set; }
}

public class InvoiceSaveDto
{
    public string InvoiceNo { get; set; } = string.Empty;
    public string TripID { get; set; } = string.Empty;
    public decimal TripAmount { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal TipAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public short PaymentType { get; set; }
    public string BookingNo { get; set; } = string.Empty;
}

public class InvoicePayDto
{
    public string InvoiceNo { get; set; } = string.Empty;
    public string TripID { get; set; } = string.Empty;
    public decimal PaidAmount { get; set; }
}

public class InvoiceMailDto
{
    public string InvoiceNo { get; set; } = string.Empty;
    public string TripID { get; set; } = string.Empty;
}

// Replaces usp_GetDriverPayments — aggregation query result
public class DriverPaymentDto
{
    public string CreatedOn { get; set; } = string.Empty;
    public decimal PaidAmount { get; set; }
}

// Per-trip invoice summary for driver accounts page (with payment type for filtering)
public class DriverTripInvoiceDto
{
    public string InvoiceNo { get; set; } = string.Empty;
    public string TripID { get; set; } = string.Empty;
    public string BookingNo { get; set; } = string.Empty;
    public DateTime TripDate { get; set; }
    public string LocationFrom { get; set; } = string.Empty;
    public string LocationTo { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public short PaymentType { get; set; }
    public bool IsPaid { get; set; }
}

// Replaces usp_GetDriverSummary — computed summary result
public class DriverSummaryDto
{
    public decimal CurrentBalance { get; set; }
    public decimal TodaySummary { get; set; }
    public int TodayBookings { get; set; }
    public decimal TodayPayment { get; set; }
    public decimal LastPayment { get; set; }
}
