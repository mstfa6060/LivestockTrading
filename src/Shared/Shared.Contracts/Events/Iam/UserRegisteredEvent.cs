using Shared.Contracts.Integration;

namespace Shared.Contracts.Events.Iam;

public record UserRegisteredEvent : IIntegrationEvent
{
    public const string Subject = "iam.user.registered";

    public Guid EventId { get; init; } = Guid.NewGuid();
    public DateTime OccurredAt { get; init; } = DateTime.UtcNow;
    public Guid UserId { get; init; }
    public string Email { get; init; } = string.Empty;
    public string UserName { get; init; } = string.Empty;
    public string? FirstName { get; init; }
    public string? Surname { get; init; }
}
