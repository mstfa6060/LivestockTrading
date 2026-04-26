using Shared.Contracts.Integration;

namespace Shared.Contracts.Events.Iam;

public record OtpSmsRequestedEvent : IIntegrationEvent
{
    public const string Subject = "iam.notification.sms";

    public Guid EventId { get; init; } = Guid.NewGuid();
    public DateTime OccurredAt { get; init; } = DateTime.UtcNow;
    public string PhoneNumber { get; init; } = string.Empty;
    public string OtpCode { get; init; } = string.Empty;
}
