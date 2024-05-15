using CorePush.Google;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
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
                /* FCM Sender (Android Device) */
                FcmSettings settings = new FcmSettings()
                {
                    SenderId = _fcmNotificationSetting.SenderId,
                    ServerKey = _fcmNotificationSetting.ServerKey
                };
                if (notificationModel.IsAndroiodDevice)
                {


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

                    /*
                        NotificationModel notificationModel = new NotificationModel();
                        notificationModel.IsAndroiodDevice = false;
                        notificationModel.DeviceId = "orderVendorDetails.Token";
                        notificationModel.Title = "Hi Kiran";
                        notificationModel.Body = "You have successfully placed you order";
                        Pushnotification.FCMPushNotification(notificationModel);
                     */

                    WebRequest tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");
                    tRequest.Method = "post";

                    //serverKey - Key from Firebase cloud messaging server  
                    ///var serverKey = "Your server key"
                    tRequest.Headers.Add(string.Format("Authorization: key={0}", settings.ServerKey));
                    //Sender Id - From firebase project setting  
                    ///var senderId = "Enter your SenderId";
                    tRequest.Headers.Add(string.Format("Sender: id={0}", settings.SenderId));
                    tRequest.ContentType = "application/json";
                    var totoken = "dB7ml8jJYEjRlWXLXEQn1X:APA91bFNIvAlKZ0v-ydnpkx6LT_Pabc9kZR3NzeeyDjJVHCfsbXLgs8clpK8qcnbhkc44eBASFu_mo-PvfEqGKRBRGMIVvgbLV1Yh2xJAm0MUUmqu3lFpOGYaXNElgtrPh6x-GpTyAz9";
                    var payload = new
                    {
                        to = totoken,
                        priority = "high",
                        content_available = true,
                        notification = new
                        {
                            body = notificationModel.Body,////firebaseModel.Data.Body,
                            title = notificationModel.Title,////firebaseModel.Data.Title,
                            badge = 1
                        },
                        data = notificationModel

                    };

                    string postbody = JsonConvert.SerializeObject(payload).ToString();
                    Byte[] byteArray = Encoding.UTF8.GetBytes(postbody);
                    tRequest.ContentLength = byteArray.Length;
                    using (Stream dataStream = tRequest.GetRequestStream())
                    {
                        dataStream.Write(byteArray, 0, byteArray.Length);
                        using (WebResponse tResponse = tRequest.GetResponse())
                        {
                            using (Stream dataStreamResponse = tResponse.GetResponseStream())
                            {
                                if (dataStreamResponse != null) using (StreamReader tReader = new StreamReader(dataStreamResponse))
                                    {
                                        String sResponseFromServer = tReader.ReadToEnd();
                                        //result.Response = sResponseFromServer;
                                    }
                            }
                        }
                    }
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
