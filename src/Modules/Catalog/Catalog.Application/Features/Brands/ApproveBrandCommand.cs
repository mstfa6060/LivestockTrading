namespace LivestockTrading.Catalog.Application.Features.Brands;

/// <summary>
/// Command for approving a seller-suggested Brand (Suggested → Approved).
/// Id-only operation; no DTO body. Not idempotent — Status != Suggested → DomainException.
/// </summary>
public sealed record ApproveBrandCommand(Guid BrandId, Guid ActorAdminId);
