namespace LivestockTrading.Catalog.Domain.Events.Public;

using Shared.Domain;

public sealed record BrandReactivated(Guid BrandId, Guid AdminUserId) : DomainEventBase;
