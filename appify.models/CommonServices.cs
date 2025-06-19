using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace appify.models
{
    public partial class CommonServices
    {

    }

    public partial class Merchant
    {
        public string email { get; set; }
        public string phone { get; set; }
        public string contact_name { get; set; }
        public string type { get; set; }
        public string legal_business_name { get; set; }
        public string business_type { get; set; }
        public Profile profile { get; set; }
        //public LegalInfo LegalInfo { get; set; }
    }

    public class Profile
    {
        public string category { get; set; }
        public string subcategory { get; set; }
        public Addresses addresses { get; set; }
    }

    public class Addresses
    {
        public Registered registered { get; set; }
    }

    public class Registered
    {
        public string street1 { get; set; }
        public string street2 { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string postal_code { get; set; }
        public string country { get; set; }
    }

    public class LegalInfo
    {
        public string Pan { get; set; }
        public string Gst { get; set; }
    }
    public partial class MerchantUpdate
    {
        public string phone { get; set; }
        public string legal_business_name { get; set; }
        public string contact_name { get; set; }
        public Profile profile { get; set; }
        //public LegalInfo LegalInfo { get; set; }
    }

    public partial class LinkedAccount
    {
        public string AccountID { get; set; }
    }

    public class StakeholderPayload
    {
        public string name { get; set; }
        public string email { get; set; }
        public Phone phone { get; set; }
        public int percentage_ownership { get; set; }
        public Relationship relationship { get; set; }
        public StakeholderAddress addresses { get; set; }
        public Kyc kyc { get; set; }
    }

    public class Phone
    {
        public string primary { get; set; }
    }
    public class Relationship
    {
        public bool director { get; set; }
        public bool executive { get; set; }
    }
    public class StakeholderAddress
    {
        public Residential residential { get; set; }
    }

    public class Residential
    {
        public string street { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string postal_code { get; set; }
        public string country { get; set; }
    }
    public class Kyc
    {
        public string pan { get; set; }
    }


    public class UpdateProductConfigPayload
    {
        public Settlements settlements { get; set; }
        public bool tnc_accepted { get; set; }
    }

    public class Settlements
    {
        public string account_number { get; set; }
        public string ifsc_code { get; set; }
        public string beneficiary_name { get; set; }
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
        public string account { get; set; }
        public decimal amount { get; set; }
        public string currency { get; set; }
        public bool on_hold { get; set; }

    }

    public class TransferRequest
    {
        public List<SplitPayment> transfers { get; set; }
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
