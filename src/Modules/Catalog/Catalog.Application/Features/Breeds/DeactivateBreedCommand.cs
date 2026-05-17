namespace LivestockTrading.Catalog.Application.Features.Breeds;

/// <summary>
/// Command for deactivating a Breed aggregate root (idempotent).
/// Id-only operation; no DTO body.
/// </summary>
public sealed record DeactivateBreedCommand(int BreedId, Guid ActorAdminId);
