namespace PickC.Modules.Notification.Domain.Interfaces;

public interface ISmsService
{
    Task<bool> SendAsync(string to, string message);
}
