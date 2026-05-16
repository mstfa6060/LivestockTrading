namespace LivestockTrading.Catalog.Domain.Events.Public;

using Shared.Domain;

public sealed record BreedReactivated(int BreedId) : DomainEventBase;
