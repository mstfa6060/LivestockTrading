namespace LivestockTrading.Catalog.Domain.Events.Internal;

using Shared.Domain;

public sealed record BrandRejected(Guid BrandId, Guid AdminUserId, string Reason) : DomainEventBase;
