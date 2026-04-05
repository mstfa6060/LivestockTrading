using System.Text.Json;
using LivestockTrading.Domain.Entities;
using LivestockTrading.Domain.Events;
using LivestockTrading.Workers.NotificationSender.Services;

namespace LivestockTrading.Workers.NotificationSender.EventHandlers;

public class ConversationCreatedNotificationHandler
{
    private readonly IPushNotificationService _pushNotificationService;
    private readonly PushTokenRepository _pushTokenRepository;
    private readonly ILogger<ConversationCreatedNotificationHandler> _logger;

    public ConversationCreatedNotificationHandler(
        IPushNotificationService pushNotificationService,
        PushTokenRepository pushTokenRepository,
        ILogger<ConversationCreatedNotificationHandler> logger)
    {
        _pushNotificationService = pushNotificationService;
        _pushTokenRepository = pushTokenRepository;
        _logger = logger;
    }

    public async Task HandleAsync(ConversationCreatedEvent conversationEvent)
    {
        _logger.LogInformation(
            "Processing ConversationCreatedEvent push notification. ConversationId: {ConversationId}, Initiator: {InitiatorId}, Recipient: {RecipientId}",
            conversationEvent.ConversationId,
            conversationEvent.InitiatorUserId,
            conversationEvent.RecipientUserId);

        var title = !string.IsNullOrEmpty(conversationEvent.InitiatorName)
            ? $"Yeni mesaj: {conversationEvent.InitiatorName}"
            : "Yeni bir konusma basladi";

        var body = !string.IsNullOrEmpty(conversationEvent.ProductTitle)
            ? $"'{conversationEvent.ProductTitle}' hakkinda yeni bir konusma"
            : !string.IsNullOrEmpty(conversationEvent.Subject)
                ? conversationEvent.Subject
                : "Birileri sizinle iletisime gecmek istiyor";

        var data = new Dictionary<string, string>
        {
            { "type", "new_conversation" },
            { "conversationId", conversationEvent.ConversationId.ToString() },
            { "initiatorId", conversationEvent.InitiatorUserId.ToString() }
        };

        if (conversationEvent.ProductId.HasValue)
        {
            data["productId"] = conversationEvent.ProductId.Value.ToString();
        }

        // Push bildirim tercihi + sessiz saatler (21:00-08:00 local) kontrolu
        if (!await _pushTokenRepository.ShouldSendPushNow(conversationEvent.RecipientUserId))
        {
            _logger.LogInformation("Push suppressed for user {UserId} (disabled or quiet hours)", conversationEvent.RecipientUserId);
        }
        else
        {
            var tokens = await _pushTokenRepository.GetActiveTokensForUser(conversationEvent.RecipientUserId);
            if (tokens.Count > 0)
            {
                var (successCount, invalidTokens) = await _pushNotificationService.SendPushWithCleanupAsync(tokens, title, body, data);
                _logger.LogInformation("Sent push notification to {SuccessCount}/{TotalCount} devices for user {UserId}",
                    successCount, tokens.Count, conversationEvent.RecipientUserId);

                if (invalidTokens.Count > 0)
                    await _pushTokenRepository.RevokeTokens(invalidTokens);
            }
            else
            {
                _logger.LogInformation("No active push tokens found for user {UserId}", conversationEvent.RecipientUserId);
            }
        }

        // In-app bildirim kaydet
        var actionData = JsonSerializer.Serialize(new
        {
            conversationId = conversationEvent.ConversationId,
            initiatorId = conversationEvent.InitiatorUserId,
            initiatorName = conversationEvent.InitiatorName,
            productId = conversationEvent.ProductId,
            productTitle = conversationEvent.ProductTitle
        });

        await _pushTokenRepository.SaveNotification(
            conversationEvent.RecipientUserId,
            title,
            body,
            NotificationType.NewMessage,
            actionData);
    }
}
