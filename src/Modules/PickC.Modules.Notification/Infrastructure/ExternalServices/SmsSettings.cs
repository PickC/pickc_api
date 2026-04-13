namespace PickC.Modules.Notification.Infrastructure.ExternalServices;

public class SmsSettings
{
    public string BaseUrl { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string From { get; set; } = string.Empty;
}
