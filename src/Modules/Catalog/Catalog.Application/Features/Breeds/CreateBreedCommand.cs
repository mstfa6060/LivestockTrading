using Shared.Contracts.Catalog.Admin;

namespace LivestockTrading.Catalog.Application.Features.Breeds;

/// <summary>
/// Command for creating a new Breed aggregate root.
/// Dto.CategoryCode resolved to parent Category via ICategoryRepository.GetByCodeAsync,
/// then Breed.Create(code, category, ...). Code and CategoryId are stable post-creation.
/// </summary>
public sealed record CreateBreedCommand(CreateBreedDto Dto, Guid ActorAdminId);
