namespace LivestockTrading.Catalog.Domain.Events.Public;

using Shared.Domain;

public sealed record BreedDeactivated(int BreedId) : DomainEventBase;
