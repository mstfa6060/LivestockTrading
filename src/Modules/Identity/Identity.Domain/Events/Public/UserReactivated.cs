namespace LivestockTrading.Identity.Domain.Events.Public;

using Shared.Domain;

public sealed record UserReactivated(Guid UserId, Guid ActorUserId) : DomainEventBase;
