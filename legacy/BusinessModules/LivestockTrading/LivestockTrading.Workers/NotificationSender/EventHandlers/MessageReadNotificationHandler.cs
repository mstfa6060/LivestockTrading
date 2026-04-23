using LivestockTrading.Domain.Events;
using LivestockTrading.Workers.NotificationSender.Services;

namespace LivestockTrading.Workers.NotificationSender.EventHandlers;

public class MessageReadNotificationHandler
{
    private readonly IPushNotificationService _pushNotificationService;
    private readonly ILogger<MessageReadNotificationHandler> _logger;

    public MessageReadNotificationHandler(
        IPushNotificationService pushNotificationService,
        ILogger<MessageReadNotificationHandler> logger)
    {
        _pushNotificationService = pushNotificationService;
        _logger = logger;
    }

    public Task HandleAsync(MessageReadEvent readEvent)
    {
        _logger.LogInformation(
            "Processing MessageReadEvent. MessageId: {MessageId}, ConversationId: {ConversationId}, ReadBy: {ReadByUserId}",
            readEvent.MessageId,
            readEvent.ConversationId,
            readEvent.ReadByUserId);

        // Read receipts are typically handled via SignalR for real-time updates
        // Push notification for read receipts is optional and usually not needed
        // This handler can be used for analytics or logging purposes

        _logger.LogDebug(
            "Message {MessageId} marked as read by user {UserId} at {ReadAt}",
            readEvent.MessageId,
            readEvent.ReadByUserId,
            readEvent.ReadAt);

        return Task.CompletedTask;
    }
}
