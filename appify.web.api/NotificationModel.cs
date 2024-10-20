using Newtonsoft.Json;

namespace appify.web.api
{
    public class NotificationModel
    {
        [JsonProperty("deviceId")]
        public string? DeviceId { get; set; }
        [JsonProperty("isAndroiodDevice")]
        public bool IsAndroiodDevice { get; set; }
        [JsonProperty("PlatformType")]
        public string? PlatformType { get; set; }
        [JsonProperty("title")]
        public string? Title { get; set; }
        [JsonProperty("body")]
        public string? Body { get; set; }
        public string? FCMSenderID { get; set; }
        public string? FCMServerKey { get; set; }
    }
    public class SMSNotificationModel
    {
        public string? Title { get; set; }
        [JsonProperty("body")]
        public string? Body { get; set; }
    }

    public class GoogleNotification
    {
        public class DataPayload
        {
            [JsonProperty("title")]
            public string? Title { get; set; }
            [JsonProperty("body")]
            public string? Body { get; set; }
        }
        [JsonProperty("priority")]
        public string Priority { get; set; } = "high";
        [JsonProperty("data")]
        public DataPayload? Data { get; set; }
        [JsonProperty("notification")]
        public DataPayload? Notification { get; set; }
    }

    public class FcmNotificationSetting
    {
        public string? SenderId { get; set; }
        public string? ServerKey { get; set; }
    }
}
