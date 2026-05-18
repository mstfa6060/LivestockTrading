using Shared.Contracts.Catalog.Admin;

namespace LivestockTrading.Catalog.Application.Features.Locations;

/// <summary>
/// Command for updating an existing Location reference entity.
/// Updates Name/NativeName/Population/DisplayOrder; code/path/level/country stable.
/// </summary>
public sealed record UpdateLocationCommand(
    int LocationId,
    UpdateLocationDto Dto,
    Guid ActorAdminId);
