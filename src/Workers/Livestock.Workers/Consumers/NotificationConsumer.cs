using Livestock.Workers.Services.Push;
using Microsoft.Extensions.Logging;
using NATS.Client.Core;
using Shared.Contracts.Events.Livestock;
using Shared.Infrastructure.Messaging;

namespace Livestock.Workers.Consumers;

public sealed class NotificationConsumer(
    INatsClient nats,
    IPushNotificationService pushService,
    ILogger<NotificationConsumer> logger) : NatsConsumerBase<MessageSentEvent>(nats)
{
    protected override string Subject => MessageSentEvent.Subject;

    protected override async Task HandleAsync(MessageSentEvent message, CancellationToken ct)
    {
        logger.LogInformation("Push notification for message {MessageId} → user {RecipientUserId}", message.MessageId, message.RecipientUserId);

        await pushService.SendAsync(
            message.RecipientUserId,
            "Yeni Mesaj",
            message.Content,
            new Dictionary<string, string>
            {
                ["conversationId"] = message.ConversationId.ToString(),
                ["messageId"] = message.MessageId.ToString(),
                ["senderUserId"] = message.SenderUserId.ToString()
            },
            ct);
    }
}
