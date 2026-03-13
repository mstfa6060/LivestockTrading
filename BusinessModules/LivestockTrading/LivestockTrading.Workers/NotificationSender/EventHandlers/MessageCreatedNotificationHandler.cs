using System.Text.Json;
using LivestockTrading.Domain.Entities;
using LivestockTrading.Domain.Events;
using LivestockTrading.Workers.NotificationSender.Services;

namespace LivestockTrading.Workers.NotificationSender.EventHandlers;

public class MessageCreatedNotificationHandler
{
    private readonly IPushNotificationService _pushNotificationService;
    private readonly PushTokenRepository _pushTokenRepository;
    private readonly ILogger<MessageCreatedNotificationHandler> _logger;

    public MessageCreatedNotificationHandler(
        IPushNotificationService pushNotificationService,
        PushTokenRepository pushTokenRepository,
        ILogger<MessageCreatedNotificationHandler> logger)
    {
        _pushNotificationService = pushNotificationService;
        _pushTokenRepository = pushTokenRepository;
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

        // Push bildirim tercihi kontrolu
        if (!await _pushTokenRepository.IsPushEnabled(messageEvent.RecipientUserId))
        {
            _logger.LogInformation("Push notifications disabled for user {UserId}, skipping", messageEvent.RecipientUserId);
        }
        else
        {
            // Alicinin push token'larini cek ve gonder
            var tokens = await _pushTokenRepository.GetActiveTokensForUser(messageEvent.RecipientUserId);
            if (tokens.Count > 0)
            {
                var (successCount, invalidTokens) = await _pushNotificationService.SendPushWithCleanupAsync(tokens, title, body, data);
                _logger.LogInformation("Sent push notification to {SuccessCount}/{TotalCount} devices for user {UserId}",
                    successCount, tokens.Count, messageEvent.RecipientUserId);

                if (invalidTokens.Count > 0)
                    await _pushTokenRepository.RevokeTokens(invalidTokens);
            }
            else
            {
                _logger.LogInformation("No active push tokens found for user {UserId}", messageEvent.RecipientUserId);
            }
        }

        // In-app bildirim kaydet
        var actionData = JsonSerializer.Serialize(new
        {
            conversationId = messageEvent.ConversationId,
            messageId = messageEvent.MessageId,
            senderId = messageEvent.SenderUserId,
            senderName = messageEvent.SenderName
        });

        await _pushTokenRepository.SaveNotification(
            messageEvent.RecipientUserId,
            title,
            body,
            NotificationType.NewMessage,
            actionData);
    }
}
