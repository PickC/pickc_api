namespace PickC.Modules.Billing.Domain.Entities;

public class Invoice
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
