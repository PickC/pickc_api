/*
 * Company: AppifyRetail.
 * Author: Gurjeet
 * Version: 1.1
 * Date: 2024-09-01
 * Description:
*/
using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using appify.Business.Contract;
using appify.models;
using appify.utility;
using Microsoft.Extensions.Configuration;
using Google.Apis.Auth.OAuth2;

namespace appify.web.api
{
    public class PushNotification
    {
        public readonly IEventLogBusiness eventLogBusiness;
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
                    ///notificationModel.IsAndroiodDevice = notificationModel2.IsAndroiodDevice;
                    notificationModel.DeviceId = notificationModel2.DeviceId;
                    notificationModel.Title = notificationModel2.Title;
                    notificationModel.Body = notificationModel2.Body;
                    notificationModel.PlatformType = notificationModel2.PlatformType;
                    service.SendNotificationAsync(notificationModel);
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
                bool IsVendor = false;
                //////Notification Template
                NotificationTemplate notificationTemplate = notificationBusiness.GetNotificationTemplate(TemplateID);
                if (VendorID == 0)
                {
                    VendorID = MemberID;
                    IsVendor = true;
                }

                VendorDetails vendorDetails = notificationBusiness.GetVendorDetails(MemberID, OrderID);
                VendorDetails FCMCredentials = notificationBusiness.GetVendorDetails(VendorID, OrderID);
                ////notificationModel.IsAndroiodDevice = true;
                notificationModel.PlatformType = vendorDetails.PlatformType;

                notificationModel.Title = notificationTemplate.MessageTitle.Replace(replaceTitle, vendorDetails.FirstName).Trim();
                if (TemplateID == 1008) ////Order Status Change //// Requested to Stop this message
                {
                    notificationModel.Body = notificationTemplate.MessageBody.Replace("#<order No>", vendorDetails.OrderNo).Trim()
                        .Replace("<Delivery Date>", vendorDetails.DeliveredOn.ToString()).Trim()
                        .Replace("<Tracking Link>", vendorDetails.TrackURL.ToString()).Trim();
                }
                else if (TemplateID == 1012) ////Refund Processed
                {
                    notificationModel.Body = notificationTemplate.MessageBody.Replace("#<order No>", vendorDetails.OrderNo).Trim()
                       .Replace("<date range>", "").Trim();
                }
                else if (TemplateID == 1013) ////Order Received //// Requested to Stop this message
                {
                    notificationModel.Body = notificationTemplate.MessageBody.Replace("#<order No>", vendorDetails.OrderNo).Trim()
                   .Replace("<Tracking Link>", "").Trim();
                }
                else if (TemplateID == 1015) ////Back-in-Stock Notification //// Requested to Stop this message
                {
                    notificationModel.Body = notificationTemplate.MessageBody.Replace("<product page link>", "").Trim();
                }
                else
                {
                    notificationModel.Body = notificationTemplate.MessageBody.Replace("#<order No>", vendorDetails.OrderNo).Trim();
                }

                notificationModel.DeviceId = vendorDetails.Token.Trim();
                //if (IsVendor == false)
                //{
                //    notificationModel.FCMSenderID = FCMCredentials.FCMSenderID.Trim();
                //    notificationModel.FCMServerKey = FCMCredentials.FCMServerKey.Trim();
                //}
                //else if(IsVendor == true)
                //{
                //    notificationModel.FCMSenderID = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("FcmNotification:SenderId").Value;
                //    notificationModel.FCMServerKey = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("FcmNotification:ServerKey").Value;
                //    VendorID = 0;
                //}
                if (IsVendor == true)
                {
                    VendorID = 0;
                }
                FCMPushNotification_New(notificationModel);

                //PushNotificationMessage pushNotificationMessage = new PushNotificationMessage
                //{ OrderID= OrderID, SenderID = VendorID, ReceiverID = MemberID, NotificationTitle = notificationModel.Title, NotificationMessage = notificationModel.Body };
                //notificationBusiness.addNotificationMessage(pushNotificationMessage);

                result = true;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

        public static async Task<string> FCMPushNotification_New(NotificationModel notificationModel2)
        {
            string returnMessage = "";
            try
            {
                if (FirebaseApp.DefaultInstance == null)
                {
                    FirebaseApp.Create(new AppOptions()
                    {
                        Credential = GoogleCredential.FromFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appify-android-gcp-new.json")),
                    });
                }
                var message = new Message()
                {
                    Notification = new Notification
                    {
                        Title = notificationModel2.Title,
                        Body = notificationModel2.Body,
                    },
                    Token = notificationModel2.DeviceId
                };

                var messaging = FirebaseMessaging.DefaultInstance;
                var result = await messaging.SendAsync(message);

                if (!string.IsNullOrEmpty(result))
                {
                    // Message was sent successfully
                    returnMessage="Message sent successfully!";
                }
                else
                {
                    // There was an error sending the message
                    returnMessage = "Error sending the message.";
                }
            }
            catch (Exception ex)
            {
                returnMessage=ex.Message;
                throw ex;

            }

            return returnMessage;
        }

    }
}
