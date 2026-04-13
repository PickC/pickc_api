using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PickC.Modules.Notification.Domain.Interfaces;

namespace PickC.Modules.Notification.Infrastructure.ExternalServices;

public class SmsService : ISmsService
{
    private readonly HttpClient _httpClient;
    private readonly SmsSettings _settings;
    private readonly ILogger<SmsService> _logger;

    public SmsService(HttpClient httpClient, IOptions<SmsSettings> settings, ILogger<SmsService> logger)
    {
        _httpClient = httpClient;
        _settings = settings.Value;
        _logger = logger;
    }

    public async Task<bool> SendAsync(string to, string message)
    {
        if (string.IsNullOrEmpty(_settings.BaseUrl))
        {
            _logger.LogWarning("SMS gateway not configured. Skipping SMS to {To}", to);
            return false;
        }

        var url = $"{_settings.BaseUrl}?username={Uri.EscapeDataString(_settings.UserName)}" +
                  $"&password={Uri.EscapeDataString(_settings.Password)}" +
                  $"&from={Uri.EscapeDataString(_settings.From)}" +
                  $"&to={Uri.EscapeDataString(to)}" +
                  $"&message={Uri.EscapeDataString(message)}";

        var response = await _httpClient.GetAsync(url);

        if (response.IsSuccessStatusCode)
        {
            _logger.LogInformation("SMS sent to {To}", to);
            return true;
        }

        _logger.LogWarning("SMS failed to {To}: {Status}", to, response.StatusCode);
        return false;
    }
}
