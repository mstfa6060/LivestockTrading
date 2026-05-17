namespace LivestockTrading.Catalog.Application.Features.Brands;

/// <summary>
/// Command for deactivating an approved Brand (Approved → Deactivated).
/// Id-only operation; no DTO body. Not idempotent — Status != Approved → DomainException.
/// Domain accepts an optional reason; port carries none → handler passes null (K4).
/// </summary>
public sealed record DeactivateBrandCommand(Guid BrandId, Guid ActorAdminId);
