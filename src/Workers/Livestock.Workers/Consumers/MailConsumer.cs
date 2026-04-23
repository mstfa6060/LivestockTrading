using Livestock.Workers.Models;
using Livestock.Workers.Services.Email;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NATS.Client.Core;
using Shared.Contracts.Events.Iam;
using Shared.Infrastructure.Messaging;

namespace Livestock.Workers.Consumers;

public sealed class MailConsumer(
    INatsClient nats,
    IEmailService emailService,
    IConfiguration config,
    ILogger<MailConsumer> logger) : NatsConsumerBase<EmailNotificationPayload>(nats)
{
    protected override string Subject => ForgotPasswordRequestedEvent.Subject; // "iam.notification.email"

    protected override async Task HandleAsync(EmailNotificationPayload payload, CancellationToken ct)
    {
        if (string.IsNullOrEmpty(payload.Email))
        {
            logger.LogWarning("Email notification received with empty email, skipping");
            return;
        }

        if (payload.Type == "password-reset")
        {
            await HandlePasswordResetAsync(payload, ct);
        }
        else if (payload.Type == "email-otp")
        {
            await HandleEmailOtpAsync(payload, ct);
        }
        else
        {
            logger.LogWarning("Unknown email notification type: {Type}", payload.Type);
        }
    }

    private async Task HandlePasswordResetAsync(EmailNotificationPayload payload, CancellationToken ct)
    {
        var frontendUrl = config["FrontendUrl"] ?? "https://globallivestock.com";
        var resetLink = $"{frontendUrl}/reset-password?token={payload.ResetToken}";
        var body = $"<p>Şifrenizi sıfırlamak için <a href=\"{resetLink}\">buraya tıklayın</a>.</p>" +
                   "<p>Bu bağlantı 1 saat geçerlidir.</p>";

        logger.LogInformation("Sending password reset email to {Email}", payload.Email);
        await emailService.SendAsync(payload.Email, "Şifre Sıfırlama", body, ct);
    }

    private async Task HandleEmailOtpAsync(EmailNotificationPayload payload, CancellationToken ct)
    {
        var body = $"<p>Doğrulama kodunuz: <strong>{payload.OtpCode}</strong></p>" +
                   "<p>Bu kod 10 dakika geçerlidir.</p>";

        logger.LogInformation("Sending email OTP to {Email}", payload.Email);
        await emailService.SendAsync(payload.Email, "Doğrulama Kodu", body, ct);
    }
}
