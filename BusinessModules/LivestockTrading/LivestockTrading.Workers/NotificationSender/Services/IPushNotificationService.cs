namespace LivestockTrading.Workers.NotificationSender.Services;

public interface IPushNotificationService
{
    Task<bool> SendPushAsync(string deviceToken, string title, string body, Dictionary<string, string>? data = null);
    Task<int> SendPushToMultipleAsync(List<string> deviceTokens, string title, string body, Dictionary<string, string>? data = null);
}
