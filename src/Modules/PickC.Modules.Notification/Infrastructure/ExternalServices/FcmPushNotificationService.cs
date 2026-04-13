using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PickC.Modules.Notification.Domain.Interfaces;

namespace PickC.Modules.Notification.Infrastructure.ExternalServices;

public class FcmPushNotificationService : IPushNotificationService
{
    private readonly ILogger<FcmPushNotificationService> _logger;

    public FcmPushNotificationService(IConfiguration config, ILogger<FcmPushNotificationService> logger)
    {
        _logger = logger;

        if (FirebaseApp.DefaultInstance is null)
        {
            var credentialPath = config["Firebase:CredentialPath"];
            if (!string.IsNullOrEmpty(credentialPath) && File.Exists(credentialPath))
            {
                FirebaseApp.Create(new AppOptions
                {
                    Credential = GoogleCredential.FromFile(credentialPath)
                });
            }
            else
            {
                _logger.LogWarning("Firebase credentials not found at '{Path}'. Push notifications disabled.", credentialPath);
            }
        }
    }

    public async Task SendAsync(string deviceToken, string bookingNo, string message)
    {
        if (FirebaseApp.DefaultInstance is null)
        {
            _logger.LogWarning("Firebase not initialized. Skipping push to {Token}", deviceToken);
            return;
        }

        var msg = new Message
        {
            Token = deviceToken,
            Data = new Dictionary<string, string>
            {
                ["bookingNo"] = bookingNo,
                ["body"] = message
            }
        };

        var response = await FirebaseMessaging.DefaultInstance.SendAsync(msg);
        _logger.LogInformation("FCM sent to {Token}: {Response}", deviceToken, response);
    }

    public async Task SendToManyAsync(List<string> deviceTokens, string bookingNo, string message)
    {
        if (FirebaseApp.DefaultInstance is null)
        {
            _logger.LogWarning("Firebase not initialized. Skipping multicast push.");
            return;
        }

        if (deviceTokens.Count == 0) return;

        var msg = new MulticastMessage
        {
            Tokens = deviceTokens,
            Data = new Dictionary<string, string>
            {
                ["bookingNo"] = bookingNo,
                ["body"] = message
            }
        };

        var response = await FirebaseMessaging.DefaultInstance.SendEachForMulticastAsync(msg);
        _logger.LogInformation("FCM multicast: {Success}/{Total} succeeded", response.SuccessCount, deviceTokens.Count);
    }
}
