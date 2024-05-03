using CorePush.Google;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using static appify.web.api.GoogleNotification;

namespace appify.web.api
{
    public class NotificationService : IDisposable
    {
        private readonly FcmNotificationSetting? _fcmNotificationSetting;
        public NotificationService(FcmNotificationSetting settings)
        {
            _fcmNotificationSetting = settings;
        }

        public async Task<ResponseMessage> SendNotificationAsync(NotificationModel notificationModel)
        {
            ResponseMessage response = new ResponseMessage();
            try
            {
                if (notificationModel.IsAndroiodDevice)
                {
                    /* FCM Sender (Android Device) */
                    FcmSettings settings = new FcmSettings()
                    {
                        SenderId = _fcmNotificationSetting.SenderId,
                        ServerKey = _fcmNotificationSetting.ServerKey
                    };

                    HttpClient httpClient = new HttpClient();

                    string authorizationKey = string.Format("key={0}", settings.ServerKey);
                    string deviceToken = notificationModel.DeviceId;

                    httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", authorizationKey);
                    httpClient.DefaultRequestHeaders.Accept
                            .Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    DataPayload dataPayload = new GoogleNotification.DataPayload();
                    dataPayload.Title = notificationModel.Title;
                    dataPayload.Body = notificationModel.Body;

                    GoogleNotification notification = new GoogleNotification();
                    notification.Data = dataPayload;
                    notification.Notification = dataPayload;

                    var fcm = new FcmSender(settings, httpClient);
                    var fcmSendResponse = await fcm.SendAsync(deviceToken, notification);

                    if (fcmSendResponse.IsSuccess())
                    {
                        response.message = "true";
                        response.data = "Notification sent successfully";
                        return response;
                    }
                    else
                    {
                        response.message = "false";
                        response.data = fcmSendResponse.Results[0].Error;
                        return response;
                    }
                }
                else
                {
                    /* Code here for APN Sender (iOS Device) */
                    //var apn = new ApnSender(apnSettings, httpClient);
                    //await apn.SendAsync(notification, deviceToken);
                }
                return response;
            }
            catch (Exception ex)
            {
                response.message = "false";
                response.data = "Something went wrong";
                return response;
            }
        }

            void IDisposable.Dispose()
            {
                if (_fcmNotificationSetting !=null && _fcmNotificationSetting is IDisposable canDispose)
                {
                    canDispose.Dispose();
                }

                GC.SuppressFinalize(this);
            }
    }
}
