using Shared.Contracts.Integration;

namespace Livestock.Workers.Models;

// Union payload covering both ForgotPasswordRequestedEvent and EmailOtpRequestedEvent,
// which share the "iam.notification.email" NATS subject.
// The Type field distinguishes the two: "password-reset" or "email-otp".
public record EmailNotificationPayload : IIntegrationEvent
{
    public Guid EventId { get; init; } = Guid.NewGuid();
    public DateTime OccurredAt { get; init; } = DateTime.UtcNow;
    public string Type { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string? ResetToken { get; init; }
    public string? OtpCode { get; init; }
}
