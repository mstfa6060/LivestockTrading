namespace LivestockTrading.Catalog.Application.Features.Categories;

/// <summary>
/// Command for deactivating a Category aggregate root (idempotent).
/// Id-only operation; no DTO body.
/// </summary>
public sealed record DeactivateCategoryCommand(int CategoryId, Guid ActorAdminId);
