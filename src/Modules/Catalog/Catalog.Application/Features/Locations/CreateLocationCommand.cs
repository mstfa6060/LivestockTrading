using Shared.Contracts.Catalog.Admin;

namespace LivestockTrading.Catalog.Application.Features.Locations;

/// <summary>
/// Command for creating a new Location reference entity (admin).
/// Slug = SlugHelper.Normalize(Code); Path = parent-chain (server-gen, Plan-4.5).
/// dto.Level (Contracts) → Domain LocationLevel via inline switch (KAYDET-22).
/// </summary>
public sealed record CreateLocationCommand(
    CreateLocationDto Dto,
    Guid ActorAdminId);
