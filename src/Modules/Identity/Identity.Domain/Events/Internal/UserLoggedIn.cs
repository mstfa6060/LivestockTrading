namespace LivestockTrading.Identity.Domain.Events.Internal;

using Shared.Domain;

public sealed record UserLoggedIn(Guid UserId, Guid DeviceId, string IpAddress) : DomainEventBase;
