namespace LivestockTrading.Catalog.Domain.Events.Internal;

using Shared.Domain;

public sealed record BreedCreated(int BreedId, string Code, int CategoryId) : DomainEventBase;
