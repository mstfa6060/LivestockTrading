namespace Livestock.Workers.Services.Push;

public interface IPushNotificationService
{
    Task SendAsync(
        Guid recipientUserId,
        string title,
        string body,
        Dictionary<string, string>? data = null,
        CancellationToken ct = default);
}
