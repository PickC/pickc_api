using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace appify.models
{
    public partial class OrderHeader
    {
        public Int64 OrderID { get; set; }

        public string OrderNo { get; set; }

        public DateTime OrderDate { get; set; }

        public Int64 VendorID { get; set; }
        public Int64 MemberID { get; set; }

        public Int16 OrderStatus { get; set; }

        public Int64 AddressID { get; set; }

        public decimal OrderAmount { get; set; }

        public decimal DiscountAmount { get; set; }

        public decimal TaxAmount { get; set; }

        public decimal TotalAmount { get; set; }

        public string Currency { get; set; }

        public decimal PaidAmount { get; set; }

        public bool IsCancel { get; set; }

        public bool IsDelivered { get; set; }

        public string Remarks { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime ModifiedOn { get; set; }

        public Int64 CancelBy { get; set; }

        public DateTime DeliveredOn { get; set; }

        public DateTime PaymentConfirmedOn { get; set; }

        public DateTime ClosedOn { get; set; }

        public DateTime CanceledOn { get; set; }

        public string DeliveryInstruction { get; set; }

        public decimal DeliveryCost { get; set; }

        public bool IsAssignPickup { get; set; }

        public decimal TotalWeight { get; set; }
        public decimal Length { get; set; }
        public decimal Width { get; set; }
        public decimal Height { get; set; }
        public string CourierRefID { get; set; }
        public string ShipmentID { get; set; }
        public string AWB { get; set; }

        public short PaymentType { get; set; }

        public string? ReceiverName { get; set; }
        public string? ReceiverMobileNo { get; set; }

        public short? DeliveryChannel { get; set; }
        public string? DeliveryChannelDescription { get; set; }



    }

    public class Order
    {

        public Int64 OrderID { get; set; }
        public string? OrderNo { get; set; }

        public DateTime OrderDate { get; set; }

        public Int64 VendorID { get; set; }
        public Int64 MemberID { get; set; }

        public string FirstName { get; set; }
        public string EmailID { get; set; }
        public string MobileNo { get; set; }

        public Int64 AddressID { get; set; }

        public decimal OrderAmount { get; set; }

        public decimal DiscountAmount { get; set; }

        public decimal TaxAmount { get; set; }

        public decimal TotalAmount { get; set; }

        public string? Currency { get; set; }

        public decimal PaidAmount { get; set; }

        public string? Remarks { get; set; }
        public string? ReceiverName { get; set; }
        public string? ReceiverMobileNo { get; set; }

        public string? DeliveryInstruction { get; set; }
        public decimal DeliveryCost { get; set; }

        public short PaymentType { get; set; }

        public bool IsSameState { get; set; }
        public short? DeliveryChannel { get; set; }
        public string? DeliveryChannelDescription { get; set; }
        public string? DeviceToken { get; set; }
        public List<OrderItem> items { get; set; }

    }

    public partial class VendorOrder
    {
        public VendorOrder()
        {
            items = new List<OrderDetail>();
        }

        public Int64 OrderID { get; set; }

        public string OrderNo { get; set; }

        public DateTime OrderDate { get; set; }

        public Int64 VendorID { get; set; }
        public Int64 MemberID { get; set; }

        public Int64? AddressID { get; set; }
        public Int16 OrderStatus { get; set; }


        public decimal OrderAmount { get; set; }

        public decimal DiscountAmount { get; set; }

        public decimal TaxAmount { get; set; }

        public decimal TotalAmount { get; set; }

        public bool IsCancel { get; set; }

        public bool IsDelivered { get; set; }

        public string Remarks { get; set; }

        public string DeliveryInstruction { get; set; }

        public decimal DeliveryCost { get; set; }

        public string OrderStatusDescription { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public short PaymentType { get; set; }
        public string PaymentTypeDescription { get; set; }

        public DateTime? DeliveredOn { get; set; }

        public string SettlementStatus { get; set; }
        public string SettlementDescription { get; set; }

        public DateTime? SettlementDate { get; set; }
        public decimal? SettlementAmount { get; set; }
        public string Reason { get; set; }

        public short? DeliveryChannel { get; set; }

        public string? DeliveryChannelDescription { get; set; }
        public string ShippingAddress { get; set; }
        public string CurrentRemarks { get; set; }
        public DateTime CurrentDate { get; set; }
        public List<OrderDetail> items { get; set; }

    }

    public partial class VendorOrderNew
    {
        public VendorOrderNew()
        {
            items = new List<OrderDetailNew>();
        }

        public Int64 OrderID { get; set; }

        public string OrderNo { get; set; }

        public DateTime OrderDate { get; set; }

        //public Int64 VendorID { get; set; }
        //public Int64 MemberID { get; set; }

        public Int64? AddressID { get; set; }
        public Int64? VAddressID { get; set; }
        public Int16 OrderStatus { get; set; }


        public decimal OrderAmount { get; set; }

        public decimal DiscountAmount { get; set; }

        public decimal TaxAmount { get; set; }

        public decimal TotalAmount { get; set; }

        //public bool IsCancel { get; set; }

        //public bool IsDelivered { get; set; }

        public string Remarks { get; set; }

        public string DeliveryInstruction { get; set; }

        public decimal DeliveryCost { get; set; }

        //public string OrderStatusDescription { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public short PaymentType { get; set; }
        //public string PaymentTypeDescription { get; set; }

        public DateTime? DeliveredOn { get; set; }

        public string SettlementStatus { get; set; }
        //public string SettlementDescription { get; set; }

        public DateTime? SettlementDate { get; set; }
        public decimal? SettlementAmount { get; set; }
        public string Reason { get; set; }

        public short? DeliveryChannel { get; set; }

        public string? DeliveryChannelDescription { get; set; }
        public string ShippingAddress { get; set; }
        public string CurrentRemarks { get; set; }
        public DateTime CurrentDate { get; set; }
        public List<OrderDetailNew> items { get; set; }

    }
    public class OrderHeaderDelivery
    {

        public OrderHeaderDelivery()
        {
            order_items = new List<OrderDetailDelivery>();
        }
        public string order_id { get; set; }
        public string order_date { get; set; }
        public string pickup_location { get; set; }
        public string pickup_address { get; set; }

        public string channel_id { get; set; }
        public string comment { get; set; }
        public string billing_customer_name { get; set; }
        public string billing_last_name { get; set; }
        public string billing_address { get; set; }
        public string billing_address_2 { get; set; }
        public string billing_city { get; set; }
        public string billing_pincode { get; set; }
        public string billing_state { get; set; }
        public string billing_country { get; set; }
        public string billing_email { get; set; }
        public string billing_phone { get; set; }
        public bool shipping_is_billing { get; set; }
        public string shipping_customer_name { get; set; }
        public string shipping_last_name { get; set; }
        public string shipping_address { get; set; }
        public string shipping_address_2 { get; set; }
        public string shipping_city { get; set; }
        public string shipping_pincode { get; set; }
        public string shipping_country { get; set; }
        public string shipping_state { get; set; }
        public string shipping_email { get; set; }
        public string shipping_phone { get; set; }
        public string payment_method { get; set; }
        public decimal shipping_charges { get; set; }
        public decimal giftwrap_charges { get; set; }
        public decimal transaction_charges { get; set; }
        public decimal total_discount { get; set; }
        public decimal sub_total { get; set; }
        public decimal length { get; set; }
        public decimal breadth { get; set; }
        public decimal height { get; set; }
        public decimal weight { get; set; }

        public List<OrderDetailDelivery> order_items { get; set; }
    }

    public partial class OrderDetailDelivery
    {

        public string name { get; set; }
        public string sku { get; set; }
        public short units { get; set; }
        public decimal selling_price { get; set; }
        public decimal discount { get; set; }
        public decimal tax { get; set; }
        public string hsn { get; set; }



    }

    public partial class OrderPayment
    {
        public Int64 PaymentID { get; set; }
        public DateTime PaymentDate { get; set; }
        public Int64 OrderID { get; set; }
        public string EventName { get; set; }
        public decimal PaymentAmount { get; set; }
        public string OrderReferenceNo { get; set; }
        public string PaymentReferenceNo { get; set; }
        public short PaymentMode { get; set; }
        public string LookupCode { get; set; }
    }
    public partial class OrderTrackingDetails
    {

        public Int64 OrderID { get; set; }
        public string CourierRefID { get; set; }
        public string ShipmentID { get; set; }
        public string AWB { get; set; }

    }
    public partial class OrderCreateWebApp
    {
        OrderCreateWebApp()
        {
            note = new List<string>();
        }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public string Receipt { get; set; }
        public List<string> note { get; set; }
        public bool PartialPayment { get; set; }
        public long FirstPaymentMinAmount { get; set; }
    }
    public partial class OrderTrackingUpdate
    {
        public string OrderNo { get; set; }
        public short OrderStatus { get; set; }
        public string Remarks { get; set; }
        public string CourierRefID { get; set; }
        public string ShipmentID { get; set; }
        public string AWB { get; set; }
        public DateTime DeliveredOn { get; set; }
        public string CourierName { get; set; }
        public string TrackURL { get; set; }
    }

    public partial class OrderTrackingUpdateDelhivery
    {
        public string AWB { get; set; }
        public string Status { get; set; }
        public string StatusType { get; set; }
        public string Instructions { get; set; }
        public string ReferenceNo { get; set; }
        public DateTime StatusDateTime { get; set; }
        public DateTime? ExpectedDeliveryDate { get; set; }
    }

    public partial class OrderUpdateDetail
    {
        public Int64 OrderID { get; set; }
        public Int64 VendorID { get; set; }
        public string VendorMobileNo { get; set; }
        public Int64 MemberID { get; set; }
        public string MemberMobileNo { get; set; }
        public bool IsEmail { get; set; }
        public bool IsEmailOpps { get; set; }
        public bool IsSMS { get; set; }
        public bool IsPush { get; set; }
        public string SkipNo { get; set; }
        public bool IsSkipNo { get; set; }
        public bool IsWhatsApp {  get; set; }
    }
    public partial class CustomerOrder
    {

        public CustomerOrder()
        {
            items = new List<OrderDetail>();
        }
        public List<OrderDetail> items { get; set; }
        public Int64 OrderID { get; set; }

        public string OrderNo { get; set; }

        public DateTime OrderDate { get; set; }

        public Int64 VendorID { get; set; }
        public Int64 MemberID { get; set; }

        public Int16 OrderStatus { get; set; }


        public decimal OrderAmount { get; set; }
        //public string OrderAmount { get; set; }

        public decimal DiscountAmount { get; set; }

        public decimal TaxAmount { get; set; }

        public decimal TotalAmount { get; set; }

        public bool IsCancel { get; set; }

        public bool IsDelivered { get; set; }

        public string Remarks { get; set; }

        public string DeliveryInstruction { get; set; }

        public decimal DeliveryCost { get; set; }

        public string OrderStatusDescription { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public short PaymentType { get; set; }
        public string PaymentTypeDescription { get; set; }


        public long AddressID { get; set; }
        public string MobileNo { get; set; }
        public string ZipCode { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string Landmark { get; set; }
        public string AlternateNo { get; set; }

        public Int64 ProductID { get; set; }
        public string ProductDescription { get; set; }
        public short? DeliveryChannel { get; set; }
        public string? DeliveryChannelDescription { get; set; }
        public bool CanCancel { get; set; }
        public string? EstimateDeliveryDate { get; set; }
        public string? OrderStatusGroup { get; set; }

    }

    public partial class CustomerOrderNew
    {

        public CustomerOrderNew()
        {
            items = new List<OrderDetailNew>();
        }
        public List<OrderDetailNew> items { get; set; }
        public Int64 OrderID { get; set; }
        public string OrderNo { get; set; }
        public DateTime OrderDate { get; set; }
        public long AddressID { get; set; }
        public long VAddressID { get; set; }
        public Int16 OrderStatus { get; set; }
        public decimal OrderAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public string Remarks { get; set; }
        public string DeliveryInstruction { get; set; }
        public decimal DeliveryCost { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public short PaymentType { get; set; }
        public DateTime? DeliveredOn { get; set; }
        public string SettlementStatus { get; set; }
        public DateTime? SettlementDate { get; set; }
        public decimal SettlementAmount { get; set; }
        public string Reason { get; set; }
        public short? DeliveryChannel { get; set; }
        public string? DeliveryChannelDescription { get; set; }
        public string? ShippingAddress { get; set; }
        public string? CurrentRemarks { get; set; }
        public DateTime? CurrentDate { get; set; }
        public string? OrderStatusGroup { get; set; }

    }
    public partial class CustomerOrderSummary
    {

        public CustomerOrderSummary()
        {
        }
        public Int64 OrderID { get; set; }

        public string OrderNo { get; set; }

        public DateTime OrderDate { get; set; }

        public Int16 OrderStatus { get; set; }


        public decimal OrderAmount { get; set; }

    }

    public class VerifyRequestModel
    {
        public string X_VERIFY { get; set; }
        public string base64 { get; set; }
        public string TransactionId { get; set; }
        public string MERCHANTID { get; set; }
        // Add other properties from the request if needed
    }

    public class PhonePeWebhookPayload
    {
        [JsonProperty("success")]
        public bool success { get; set; }

        [JsonProperty("code")]
        public string code { get; set; }

        [JsonProperty("message")]
        public string message { get; set; }

        [JsonProperty("data")]
        public merchantdata data { get; set; }

        public class merchantdata
        {
            [JsonProperty("merchantId")]
            public string merchantId { get; set; }

            [JsonProperty("merchantTransactionId")]
            public string merchantTransactionId { get; set; }

            [JsonProperty("instrumentResponse")]
            public instrumentData instrumentResponse { get; set; }
        }

        public class instrumentData
        {
            [JsonProperty("type")]
            public string type { get; set; }

            [JsonProperty("redirectInfo")]
            public redirectData redirectInfo { get; set; }
        }
        public class redirectData
        {
            [JsonProperty("url")]
            public string url { get; set; }

            [JsonProperty("method")]
            public string method { get; set; }
        }

        // Add other relevant fields based on PhonePe's webhook payload structure
    }

    public class RazorpayWebhookPayload
    {
        [JsonProperty("event")]
        public string Event { get; set; }

        [JsonProperty("payload")]
        public PayloadData Payload { get; set; }

        public class PayloadData
        {
            [JsonProperty("payment")]
            public PaymentData Payment { get; set; }
        }

        public class PaymentData
        {
            [JsonProperty("entity")]
            public PaymentEntity Entity { get; set; }
        }

        public class PaymentEntity
        {
            [JsonProperty("id")]
            public string Id { get; set; }

            [JsonProperty("amount")]
            public int Amount { get; set; }

            [JsonProperty("currency")]
            public string Currency { get; set; }

            [JsonProperty("status")]
            public string Status { get; set; }

            [JsonProperty("order_id")]
            public string OrderId { get; set; }

            // Add other relevant fields based on Razorpay's webhook payload structure
        }

    }
    public class DailyOrderSummary
    {
        public int OrderID { get; set; }
        public string OrderNo { get; set; }
        public DateTime OrderDate { get; set; }
        public string VendorName { get; set; }
        public string CustomerName { get; set; }
        public string MobileNo { get; set; }
        public string EmailID { get; set; }
        public string OrderStatus { get; set; }
        public decimal OrderAmount { get; set; }
        public string PaymentType { get; set; }
    }
    public class OrderList
    {
        public int OrderID { get; set; }
        public string OrderNo { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime OrderPlacedDate { get; set; }
        
        public Int32 OrderStatus { get; set; }
        public string OrderStatusDescription { get; set; }
        public Int32 ProductID { get; set; }
        public string ProductDescription { get; set; }
        public string ImageName { get; set; }

        public string? EstimatedDeliveryDate { get; set; }
        public string Size { get; set; }
        public int Quantity { get; set; }
        public decimal? DiscountPrice { get; set; }
        public decimal? ActualPrice { get; set; }
        public decimal? OrderPrice { get; set; }

        public string? PaymentType { get; set; }
        public string PaymentReferenceNo { get; set; }
        public string? OrderStatusGroup { get; set; }
    }

}
