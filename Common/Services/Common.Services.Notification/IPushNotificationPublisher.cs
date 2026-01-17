namespace Common.Services.Notification;

public interface IPushNotificationPublisher
{
    Task SendPushAsync(string userId, string title, string body, string jobId);
    Task SendPushToTokenAsync(string token, string title, string body, string jobId);
    Task SendPushToMultipleTokensAsync(List<string> tokens, string title, string body, string jobId, Dictionary<string, string>? additionalData = null);
}
