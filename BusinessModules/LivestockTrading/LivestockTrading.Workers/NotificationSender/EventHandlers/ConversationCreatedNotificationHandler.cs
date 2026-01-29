using LivestockTrading.Domain.Events;
using LivestockTrading.Workers.NotificationSender.Services;

namespace LivestockTrading.Workers.NotificationSender.EventHandlers;

public class ConversationCreatedNotificationHandler
{
    private readonly IPushNotificationService _pushNotificationService;
    private readonly ILogger<ConversationCreatedNotificationHandler> _logger;

    public ConversationCreatedNotificationHandler(
        IPushNotificationService pushNotificationService,
        ILogger<ConversationCreatedNotificationHandler> logger)
    {
        _pushNotificationService = pushNotificationService;
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

        _logger.LogInformation(
            "Push notification prepared - Title: {Title}, Body: {Body}, RecipientUserId: {RecipientUserId}",
            title,
            body,
            conversationEvent.RecipientUserId);

        // TODO: Fetch recipient's push tokens from database and send
        // This requires integration with IAM module's UserPushToken table

        await Task.CompletedTask;
    }
}
