using Shared.Contracts.Catalog.Admin;

namespace LivestockTrading.Catalog.Application.Features.Categories;

/// <summary>
/// Command for updating an existing Category aggregate root.
/// Updates Name (Translations), Description (Translations?), DisplayOrder, IconKey
/// in a single atomic operation. Code and ParentId are stable post-creation.
/// </summary>
public sealed record UpdateCategoryCommand(int CategoryId, UpdateCategoryDto Dto, Guid ActorAdminId);
