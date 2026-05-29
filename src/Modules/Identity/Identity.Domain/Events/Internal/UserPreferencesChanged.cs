namespace LivestockTrading.Identity.Domain.Events.Internal;

using Shared.Domain;

public sealed record UserPreferencesChanged(Guid UserId) : DomainEventBase;
