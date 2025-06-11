using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace appify.models
{
    public partial class CommonServices
    {

    }

    public partial class Merchant
    {
        public string RazorPayAccountID {  get; set; }
        public string EmailID { get; set; }
        public string Phone {  get; set; }
        public string LegalBusinessName {  get; set; }
        public string BusinessType { get; set; }
        //public string ProfileCategoryName {  get; set; }
        //public string ProfileSubCategoryName { get; set; }
        public string GST {  get; set; }
        public string PAN { get; set; }
        public string IFSCCODE { get; set; }
        public string BankAccountNo {  get; set; }
        public string BeneficiaryName {  get; set; }
    }
    public partial class DelhiveryShipmentCost
    {
        public string StartPin { get; set; }
        public string DestPin { get; set; }
        public decimal Weight { get; set; }
        public string PayType {  get; set; }
        public decimal CODAmount { get; set; }

    }
    public partial class DeliveryPinCode
    {
        public string PinCode { get; set; }
    }
    public partial class CreateLinkedAccountRequest
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Contact { get; set; }
        public string LegalBusinessName { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; } = "IN";
    }
    public partial class CreateOrderLinkRequest
    {
        public int AmountInPaise { get; set; }
        public string ReceiptId { get; set; }
        public string Description { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerContact { get; set; }
        public string CallbackUrl { get; set; }
    }

    public partial class SplitPayment
    {
        public string PaymentId { get; set; }
        public decimal TotalAmount { get; set; }
        public string Currency { get; set; }
        public bool OnHold { get; set; }
        public string AccountId { get; set; }
    }
    public partial class CapturePaymentRequest
    {
        public string RazorpayPaymentId { get; set; }
    }

    public partial class CaptureTransactionRequest
    {
        public string RazorpayTransactionId { get; set; }
    }
    public partial class VerifyPaymentRequest
    {
        public string RazorpayOrderId { get; set; }
        public string RazorpayPaymentId { get; set; }
        public string RazorpaySignature { get; set; }
    }

}
