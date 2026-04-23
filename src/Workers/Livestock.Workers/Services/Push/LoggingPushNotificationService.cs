using Microsoft.Extensions.Logging;

namespace Livestock.Workers.Services.Push;

public sealed class LoggingPushNotificationService(ILogger<LoggingPushNotificationService> logger) : IPushNotificationService
{
    public Task SendAsync(Guid recipientUserId, string title, string body, Dictionary<string, string>? data = null, CancellationToken ct = default)
    {
        logger.LogInformation("[PUSH] UserId={UserId} Title={Title} Body={Body}", recipientUserId, title, body);
        return Task.CompletedTask;
    }
}
