using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace appify.models
{
    public partial class Notifications
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public Notifications() { }

        public string ToEmail { get; set; }
        public string ToEmailCC { get; set; }
        public string ToEmailBCC { get; set; }
        public string EmailSubject { get; set; }
        public string EmailTemplateURL { get; set; }
        public string EmailBody { get; set; }
    }

    public class PushNotificationMessage
    {
        public Int64 NotificationID { get; set; }
        public Int64 OrderID {  get; set; }
        public Int64 SenderID { get; set; }
        public Int64 ReceiverID { get; set; }
        public DateTime NotificationDate { get; set; }
        public string NotificationTitle { get; set; }
        public string NotificationMessage { get; set; }

        public Int16 NotificationEvent { get; set; }
        public bool IsRead { get; set; }

        public Int16 NotificationStatus { get; set; }
        public DateTime ReadOn { get; set; }
        public bool IsCancel { get; set; }

    }

    public class NotificationTemplate
    {
        public Int64 TemplateID { get; set; }
        public string Duration { get; set; }
        public string TriggerEvent { get; set; }
        public bool IsPushNotification { get; set; }
        public string MessageTitle { get; set; }
        public string MessageBody { get; set; }
        public bool IsActive { get; set; }

    }

    public class VendorDetails
    {
        public Int64 VendorID { get; set; }
        public string FirstName { get; set; }
        public string? Token { get; set; }
        public string? PlatformType { get; set; }
        public string OrderNo { get; set; }
        public string FCMServerKey { get; set; }
        public string FCMSenderID { get; set; }
        public DateTime DeliveredOn { get; set; }
        public string TrackingNumber { get; set; }
        public string TrackURL { get; set; }
    }
    public class NotificationType
    {
        public enum NotificationTemplateType
        {
            SuccessfulSignupVendor = 1000,
            SuccessfulSignupCustomer=1001,
            SuccessfulSignupOpps = 1002,
            OrderPlacementVendor = 1003,
            OrderPlacementCustomer = 1004,
            OrderPlacementOpps = 1005,
            OrderConfirmationVendor = 1006,
            OrderConfirmationCustomer = 1007,
            OrderConfirmationOpps = 1008,
            OrderCancellationVendor = 1009,
            OrderCancellationVendorCustomer = 1010,
            OrderCancellationVendorOpps = 1011,
            OrderCancellationCustomerVendor = 1012, 
            OrderCancellationCustomer = 1013,
            OrderCancellationCustomerOpps = 1014,
            //RefundProcessed = 1012,
            //OrderReceived = 1013,
            //Feedbackorrating = 1014,
            //BackinStock = 1015,

            //ShippingDeliveryUpdates = 1004,
            //DeliveryUpdates = 1005,
            //DeliveryConfirmation = 1006,
            //DelayedShipmentNotification = 1007,
            //OrderStatusChange = 1008,
            //AbandonedCartReminder = 1009,
        }
        public enum PushNotificationTemplateType
        {
            SuccessfulSignup = 1000,
            OrderPlacementCustomer = 1001,
            OrderPlacementVendor = 1002,
            OrderConfirmation = 1003,
            ShippingDeliveryUpdates = 1004,
            DeliveryUpdates = 1005,
            DeliveryConfirmation = 1006,
            DelayedShipmentNotification = 1007,
            OrderStatusChange = 1008,
            AbandonedCartReminder = 1009,
            OrderCancellationCustomer = 1010,
            OrderCancellationVendor = 1011,
            RefundProcessed = 1012,
            OrderReceived = 1013,
            Feedbackorrating = 1014,
            BackinStock = 1015,
        }
    }

    public class EmailNotificationTemplate
    {
        public Int64 TemplateID { get; set; }
        public string Duration { get; set; }
        public string TriggerEvent { get; set; }
        public bool IsEmail { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string TemplateURL { get; set; }
        public bool IsActive { get; set; }

    }

    public class EmailNotificationHeader
    {
        public Int64 VendorID { get; set; }
        public Int64 MemberID { get; set; }
        public string FirstName { get; set; }
        public string EmailID { get; set; }
        public string MobileNo { get; set; }
        public Int16 MemberType {get;set;}
        public Int64 AddressID { get; set; }
        public Int64 OrderID { get; set; }
        public string? OrderNo { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string ProductName { get; set; }
        public string AppName { get; set; }
        public string TrackingNumber { get; set; }
        public string Remarks { get; set; }
        public DateTime? DeliveredOn { get; set; }
        public short OrderQuantity { get; set; }
        public decimal OrderPrice { get; set; }
        public string shipping_address { get; set; }
        public string shipping_address_2 { get; set; }
        public string CompanyAddress1 { get; set; }
        public string CompanyAddress2 { get; set; }
        public short? DeliveryChannel { get; set; }
        public string? DeliveryChannelDescription { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerEmail { get; set; }
        public string CourierName { get; set; }
        public string TrackURL { get; set; }
    }
}