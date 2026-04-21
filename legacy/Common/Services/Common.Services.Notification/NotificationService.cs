using Microsoft.Extensions.Configuration;

namespace Common.Services.Notification;

public class NotificationService
{
    private readonly IPushNotificationFactory _factory;
    private readonly IConfiguration _configuration;

    public NotificationService(IPushNotificationFactory factory, IConfiguration configuration)
    {
        _factory = factory;
        _configuration = configuration;
    }

    private IPushNotificationPublisher GetPublisher()
    {
        var providerName = _configuration["Notification:Provider"] ?? "OneSignal";

        if (Enum.TryParse<NotificationProvider>(providerName, true, out var provider))
            return _factory.CreatePublisher(provider);

        return _factory.CreatePublisher(NotificationProvider.OneSignal);
    }

    public Task SendJobNotificationAsync(string userId, string title, string body, string jobId)
    {
        var publisher = GetPublisher();
        return publisher.SendPushAsync(userId, title, body, jobId);
    }

    public Task SendJobNotificationToTokenAsync(string token, string title, string body, string jobId)
    {
        var publisher = GetPublisher();
        return publisher.SendPushToTokenAsync(token, title, body, jobId);
    }

    public Task SendJobNotificationToMultipleTokensAsync(List<string> tokens, string title, string body, string jobId)
    {
        var publisher = GetPublisher();
        return publisher.SendPushToMultipleTokensAsync(tokens, title, body, jobId);
    }
}
