using appify.Business.Contract;
using appify.models;
using appify.utility;
using Microsoft.Extensions.Configuration;

namespace appify.web.api
{
    public class Pushnotification
    {
        public static bool FCMPushNotification(NotificationModel notificationModel2)
        {
            bool result = false;
            try
            {
                FcmNotificationSetting fcmNotificationSetting = new FcmNotificationSetting();
                NotificationModel notificationModel = new NotificationModel();

                fcmNotificationSetting.ServerKey = notificationModel2.FCMServerKey;
                ////new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("FcmNotification:ServerKey").Value;
                fcmNotificationSetting.SenderId = notificationModel2.FCMSenderID;
                ////new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("FcmNotification:SenderId").Value;

                using (NotificationService service = new NotificationService(fcmNotificationSetting))
                {
                    notificationModel.IsAndroiodDevice = notificationModel2.IsAndroiodDevice;
                    notificationModel.DeviceId = notificationModel2.DeviceId;
                    notificationModel.Title = notificationModel2.Title;
                    notificationModel.Body = notificationModel2.Body;
                    service.SendNotificationAsync(notificationModel2);
                    result = true;
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }

            return result;
        }

        public static bool SendNotificationMessage(Int64 TemplateID, Int64 VendorID, Int64 MemberID, Int64 OrderID, string replaceTitle, INotificationBusiness notificationBusiness)
        {
            bool result = false;
            try
            {
                NotificationModel notificationModel = new NotificationModel();
                //////Notification Template
                NotificationTemplate notificationTemplate = notificationBusiness.GetNotificationTemplate(TemplateID);
                VendorDetails vendorDetails = notificationBusiness.GetVendorDetails(MemberID, OrderID);
                notificationModel.IsAndroiodDevice = true;

                notificationModel.Title = notificationTemplate.MessageTitle.Replace(replaceTitle, vendorDetails.FirstName).Trim();
                if (TemplateID == 1007) ////Order Status Change
                {
                    notificationModel.Body = notificationTemplate.MessageBody.Replace("#<order No>", vendorDetails.OrderNo).Trim()
                        .Replace("<Delivery Date>", "").Trim()
                        .Replace("<Tracking Link>", "").Trim();
                }
                else if (TemplateID == 1010) ////Refund Processed
                {
                    notificationModel.Body = notificationTemplate.MessageBody.Replace("#<order No>", vendorDetails.OrderNo).Trim()
                       .Replace("<date range>", "").Trim();
                }
                else if (TemplateID == 1011) ////Order Received
                {
                    notificationModel.Body = notificationTemplate.MessageBody.Replace("#<order No>", vendorDetails.OrderNo).Trim()
                   .Replace("<Tracking Link>", "").Trim();
                }
                else if (TemplateID == 1013) ////Back-in-Stock Notification
                {
                    notificationModel.Body = notificationTemplate.MessageBody.Replace("<product page link>", "").Trim();
                }
                else
                {
                    notificationModel.Body = notificationTemplate.MessageBody.Replace("#<order No>", vendorDetails.OrderNo).Trim();
                }

                notificationModel.DeviceId = vendorDetails.Token;
                notificationModel.FCMSenderID = vendorDetails.FCMSenderID;
                notificationModel.FCMServerKey = vendorDetails.FCMServerKey;
                FCMPushNotification(notificationModel);

                PushNotificationMessage pushNotificationMessage = new PushNotificationMessage
                { SenderID = VendorID, ReceiverID = MemberID, NotificationTitle = notificationModel.Title, NotificationMessage = notificationModel.Body };
                notificationBusiness.addNotificationMessage(pushNotificationMessage);

                result = true;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

    }
}
