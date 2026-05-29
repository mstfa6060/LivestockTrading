namespace LivestockTrading.Identity.Domain.Events.Internal;

using Shared.Domain;

public sealed record UserRoleGranted(Guid UserId, string Role, Guid? GrantedByUserId) : DomainEventBase;
