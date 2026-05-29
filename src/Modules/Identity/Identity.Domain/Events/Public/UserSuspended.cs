namespace LivestockTrading.Identity.Domain.Events.Public;

using Shared.Domain;

public sealed record UserSuspended(Guid UserId, string Reason, DateTimeOffset? SuspendedUntil, Guid ActorUserId) : DomainEventBase;
