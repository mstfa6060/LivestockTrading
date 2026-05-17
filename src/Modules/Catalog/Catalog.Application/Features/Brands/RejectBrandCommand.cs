namespace LivestockTrading.Catalog.Application.Features.Brands;

/// <summary>
/// Command for rejecting a seller-suggested Brand (Suggested → Rejected).
/// Reason is required; not idempotent — Status != Suggested → DomainException.
/// </summary>
public sealed record RejectBrandCommand(Guid BrandId, string Reason, Guid ActorAdminId);
