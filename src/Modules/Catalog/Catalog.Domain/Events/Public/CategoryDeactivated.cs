namespace LivestockTrading.Catalog.Domain.Events.Public;

using Shared.Domain;

public sealed record CategoryDeactivated(int CategoryId) : DomainEventBase;
