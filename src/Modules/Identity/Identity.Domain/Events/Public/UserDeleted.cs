namespace LivestockTrading.Identity.Domain.Events.Public;

using Shared.Domain;

public sealed record UserDeleted(Guid UserId, DateTimeOffset DeletedAt) : DomainEventBase;
