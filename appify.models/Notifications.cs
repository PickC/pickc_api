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
        public string ToEmailCC {  get; set; }
        public string ToEmailBCC {  get; set; }
        public string EmailSubject {  get; set; }
        public string EmailTemplateURL {  get; set; }
        public string EmailTemplae_ReplaceName {  get; set; }
    }

    public class PushNotificationMessage
    {
        public Int64 NotificationID { get; set; }
        public Int64 SenderID { get; set; }
        public Int64 ReceiverID { get; set; }
        public DateTime NotificationDate { get; set; }
        public string NotificationTitle { get; set; }
        public string NotificationMessage { get; set; }

        public Int16 NotificationEvent{ get; set; }
        public bool IsRead {  get; set; }

        public Int16 NotificationStatus {  get; set; }
        public DateTime ReadOn { get; set; }
        public bool IsCancel { get; set; }

    }

    public class NotificationTemplate
    {
        public Int64 NotificationID { get; set; }
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
    }
    public class NotificationType
    {
        public enum NotificationTemplateType
        {
            SuccessfulSignup =1000,
            OrderConfirmation = 1001,
            OrderPlacement = 1002,
            ShippingDeliveryUpdates = 1003,
            DeliveryUpdates = 1004,
            DeliveryConfirmation = 1005,
            DelayedShipmentNotification = 1006,
            OrderStatusChange = 1007,
            AbandonedCartReminder = 1008,
            OrderCancellation = 1009,
            RefundProcessed = 1010,
            OrderReceived = 1011,
            Feedbackorrating = 1012,
            BackinStock = 1013,
        }
    }

}
