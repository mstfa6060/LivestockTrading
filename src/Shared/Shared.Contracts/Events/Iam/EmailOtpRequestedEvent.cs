using Shared.Contracts.Integration;

namespace Shared.Contracts.Events.Iam;

public record EmailOtpRequestedEvent : IIntegrationEvent
{
    public const string Subject = "iam.notification.email";

    public Guid EventId { get; init; } = Guid.NewGuid();
    public DateTime OccurredAt { get; init; } = DateTime.UtcNow;
    public string Email { get; init; } = string.Empty;
    public string OtpCode { get; init; } = string.Empty;
    public string Type { get; init; } = "email-otp";
}
