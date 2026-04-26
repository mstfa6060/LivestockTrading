using Livestock.Workers.Services.Sms;
using Microsoft.Extensions.Logging;
using NATS.Client.Core;
using Shared.Contracts.Events.Iam;
using Shared.Infrastructure.Messaging;

namespace Livestock.Workers.Consumers;

public sealed class SmsConsumer(
    INatsClient nats,
    ISmsService smsService,
    ILogger<SmsConsumer> logger) : NatsConsumerBase<OtpSmsRequestedEvent>(nats)
{
    protected override string Subject => OtpSmsRequestedEvent.Subject; // "iam.notification.sms"

    protected override async Task HandleAsync(OtpSmsRequestedEvent message, CancellationToken ct)
    {
        logger.LogInformation("Sending OTP SMS to {Phone}", message.PhoneNumber);
        await smsService.SendAsync(message.PhoneNumber, $"Doğrulama kodunuz: {message.OtpCode}", ct);
    }
}
