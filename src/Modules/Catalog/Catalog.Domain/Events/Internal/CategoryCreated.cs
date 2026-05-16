namespace LivestockTrading.Catalog.Domain.Events.Internal;

using Shared.Domain;

public sealed record CategoryCreated(int CategoryId, string Code, int Level, int? ParentId) : DomainEventBase;
