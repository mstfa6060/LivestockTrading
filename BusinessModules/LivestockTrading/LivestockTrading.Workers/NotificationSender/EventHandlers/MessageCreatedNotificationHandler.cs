using LivestockTrading.Domain.Events;
using LivestockTrading.Workers.NotificationSender.Services;

namespace LivestockTrading.Workers.NotificationSender.EventHandlers;

public class MessageCreatedNotificationHandler
{
    private readonly IPushNotificationService _pushNotificationService;
    private readonly ILogger<MessageCreatedNotificationHandler> _logger;

    public MessageCreatedNotificationHandler(
        IPushNotificationService pushNotificationService,
        ILogger<MessageCreatedNotificationHandler> logger)
    {
        _pushNotificationService = pushNotificationService;
        _logger = logger;
    }

    public async Task HandleAsync(MessageCreatedEvent messageEvent)
    {
        _logger.LogInformation(
            "Processing MessageCreatedEvent push notification. MessageId: {MessageId}, ConversationId: {ConversationId}, Sender: {SenderId}, Recipient: {RecipientId}",
            messageEvent.MessageId,
            messageEvent.ConversationId,
            messageEvent.SenderUserId,
            messageEvent.RecipientUserId);

        var title = !string.IsNullOrEmpty(messageEvent.SenderName)
            ? $"Yeni mesaj: {messageEvent.SenderName}"
            : "Yeni mesaj aldiniz";

        var body = messageEvent.Content?.Length > 100
            ? messageEvent.Content.Substring(0, 100) + "..."
            : messageEvent.Content ?? "Yeni mesajiniz var";

        var data = new Dictionary<string, string>
        {
            { "type", "new_message" },
            { "messageId", messageEvent.MessageId.ToString() },
            { "conversationId", messageEvent.ConversationId.ToString() },
            { "senderId", messageEvent.SenderUserId.ToString() }
        };

        // Log the notification details
        _logger.LogInformation(
            "Push notification prepared - Title: {Title}, Body: {Body}, RecipientUserId: {RecipientUserId}",
            title,
            body,
            messageEvent.RecipientUserId);

        // TODO: Fetch recipient's push tokens from database and send
        // This requires integration with IAM module's UserPushToken table
        // For now, we prepare the notification payload for future integration

        // Example implementation when tokens are available:
        // var tokens = await GetUserPushTokens(messageEvent.RecipientUserId);
        // if (tokens.Any())
        // {
        //     var successCount = await _pushNotificationService.SendPushToMultipleAsync(tokens, title, body, data);
        //     _logger.LogInformation("Sent push notification to {SuccessCount} devices for user {UserId}", successCount, messageEvent.RecipientUserId);
        // }

        await Task.CompletedTask;
    }
}
