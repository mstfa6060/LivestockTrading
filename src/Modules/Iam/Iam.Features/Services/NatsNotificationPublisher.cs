using Shared.Contracts.Events.Iam;
using Shared.Infrastructure.Messaging;

namespace Iam.Features.Services;

public sealed class NatsNotificationPublisher(IEventPublisher publisher) : INotificationPublisher
{
    public async Task PublishOtpAsync(string phoneNumber, string otpCode, CancellationToken ct = default)
    {
        await publisher.PublishAsync(OtpSmsRequestedEvent.Subject, new OtpSmsRequestedEvent
        {
            PhoneNumber = phoneNumber,
            OtpCode = otpCode
        }, ct);
    }

    public async Task PublishEmailOtpAsync(string email, string otpCode, CancellationToken ct = default)
    {
        await publisher.PublishAsync(EmailOtpRequestedEvent.Subject, new EmailOtpRequestedEvent
        {
            Email = email,
            OtpCode = otpCode
        }, ct);
    }

    public async Task PublishPasswordResetAsync(string email, string resetToken, CancellationToken ct = default)
    {
        await publisher.PublishAsync(ForgotPasswordRequestedEvent.Subject, new ForgotPasswordRequestedEvent
        {
            Email = email,
            ResetToken = resetToken
        }, ct);
    }
}
