using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace Common.Services.Notification;

public class OneSignalPushNotificationPublisher : IPushNotificationPublisher
{
    private readonly HttpClient _httpClient;
    private readonly string _appId;
    private readonly string _apiKey;

    public OneSignalPushNotificationPublisher(IConfiguration config)
    {
        _httpClient = new HttpClient();
        _appId = config["OneSignal:AppId"];
        _apiKey = config["OneSignal:ApiKey"];
    }

    public async Task SendPushAsync(string userId, string title, string body, string jobId)
    {
        var payload = new
        {
            app_id = _appId,
            headings = new { en = title },
            contents = new { en = body },
            data = new { type = "job-detail", jobId },
            include_external_user_ids = userId != null ? new[] { userId } : null,
            included_segments = userId == null ? new[] { "All" } : null,
            channel_for_external_user_ids = "push"
        };

        var request = new HttpRequestMessage(HttpMethod.Post, "https://onesignal.com/api/v1/notifications")
        {
            Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json")
        };

        request.Headers.Add("Authorization", $"Basic {_apiKey}");
        await _httpClient.SendAsync(request);
    }

    public Task SendPushToTokenAsync(string token, string title, string body, string jobId)
        => SendPushAsync(token, title, body, jobId);

    public async Task SendPushToMultipleTokensAsync(List<string> tokens, string title, string body, string jobId, Dictionary<string, string>? additionalData = null)
    {
        // Base data dictionary oluştur
        var data = new Dictionary<string, object>
        {
            { "type", "job-detail" },
            { "jobId", jobId }
        };

        // additionalData varsa birleştir
        if (additionalData != null)
        {
            foreach (var kvp in additionalData)
            {
                data[kvp.Key] = kvp.Value;
            }
        }

        var payload = new
        {
            app_id = _appId,
            headings = new { en = title },
            contents = new { en = body },
            data = data,
            include_external_user_ids = tokens.ToArray(),
            channel_for_external_user_ids = "push"
        };

        var request = new HttpRequestMessage(HttpMethod.Post, "https://onesignal.com/api/v1/notifications")
        {
            Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json")
        };

        request.Headers.Add("Authorization", $"Basic {_apiKey}");
        await _httpClient.SendAsync(request);
    }
}
