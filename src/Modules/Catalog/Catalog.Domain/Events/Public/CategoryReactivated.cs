namespace LivestockTrading.Catalog.Domain.Events.Public;

using Shared.Domain;

public sealed record CategoryReactivated(int CategoryId) : DomainEventBase;
