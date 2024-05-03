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

                fcmNotificationSetting.ServerKey = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("FcmNotification:ServerKey").Value;
                fcmNotificationSetting.SenderId = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("FcmNotification:SenderId").Value;

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

    }
}
