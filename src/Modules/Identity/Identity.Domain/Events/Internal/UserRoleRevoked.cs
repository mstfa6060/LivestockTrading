namespace LivestockTrading.Identity.Domain.Events.Internal;

using Shared.Domain;

public sealed record UserRoleRevoked(Guid UserId, string Role, Guid RevokedByUserId) : DomainEventBase;
