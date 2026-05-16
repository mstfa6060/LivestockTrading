namespace LivestockTrading.Catalog.Domain.Events.Internal;

using Shared.Domain;

public sealed record CategoryRenamed(int CategoryId) : DomainEventBase;
