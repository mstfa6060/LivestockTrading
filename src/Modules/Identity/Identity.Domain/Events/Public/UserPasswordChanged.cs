namespace LivestockTrading.Identity.Domain.Events.Public;

using Shared.Domain;

public sealed record UserPasswordChanged(Guid UserId, DateTimeOffset ChangedAt, string IpAddress) : DomainEventBase;
