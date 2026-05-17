using Shared.Contracts.Catalog.Admin;

namespace LivestockTrading.Catalog.Application.Features.Breeds;

/// <summary>
/// Command for updating an existing Breed aggregate root.
/// Updates Name (Translations), Description (Translations?), DisplayOrder in a single
/// atomic operation. Code and CategoryId are stable post-creation.
/// </summary>
public sealed record UpdateBreedCommand(int BreedId, UpdateBreedDto Dto, Guid ActorAdminId);
