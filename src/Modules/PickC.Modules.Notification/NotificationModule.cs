using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PickC.Modules.Notification.Domain.Interfaces;
using PickC.Modules.Notification.Infrastructure.ExternalServices;

namespace PickC.Modules.Notification;

public static class NotificationModule
{
    public static IServiceCollection AddNotificationModule(
        this IServiceCollection services, IConfiguration configuration)
    {
        // FCM Push Notifications (Firebase Admin SDK)
        services.AddSingleton<IPushNotificationService, FcmPushNotificationService>();

        // SMS Service (typed HttpClient)
        services.Configure<SmsSettings>(configuration.GetSection("Sms"));
        services.AddHttpClient<ISmsService, SmsService>();

        return services;
    }
}
