namespace LivestockTrading.Catalog.Domain.Events.Internal;

using Shared.Domain;

public sealed record BrandSuggested(Guid BrandId, string Slug, Guid SellerUserId) : DomainEventBase;
