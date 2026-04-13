namespace PickC.Modules.Notification.Domain.Interfaces;

public interface IPushNotificationService
{
    Task SendAsync(string deviceToken, string bookingNo, string message);
    Task SendToManyAsync(List<string> deviceTokens, string bookingNo, string message);
}
