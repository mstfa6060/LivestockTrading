namespace LivestockTrading.Catalog.Domain.Events.Public;

using Shared.Domain;

public sealed record BrandDeactivated(Guid BrandId, Guid AdminUserId) : DomainEventBase;
