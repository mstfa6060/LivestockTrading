namespace LivestockTrading.Catalog.Domain.Events.Public;

using Shared.Domain;

public sealed record BrandApproved(Guid BrandId, Guid AdminUserId, string Slug) : DomainEventBase;
