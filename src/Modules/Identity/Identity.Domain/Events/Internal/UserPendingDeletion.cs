namespace LivestockTrading.Identity.Domain.Events.Internal;

using Shared.Domain;

public sealed record UserPendingDeletion(Guid UserId, DateTimeOffset RequestedAt, DateTimeOffset ScheduledDeletionAt) : DomainEventBase;
