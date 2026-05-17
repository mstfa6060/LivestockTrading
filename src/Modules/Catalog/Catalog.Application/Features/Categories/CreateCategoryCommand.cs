using Shared.Contracts.Catalog.Admin;

namespace LivestockTrading.Catalog.Application.Features.Categories;

/// <summary>
/// Command for creating a new Category aggregate root (top-level or subcategory).
/// Top-level: Dto.ParentCode null → Category.CreateTopLevel(...)
/// Subcategory: Dto.ParentCode non-null → resolve parent via ICategoryRepository.GetByCodeAsync,
/// then Category.CreateSubcategory(parent, ...)
/// </summary>
public sealed record CreateCategoryCommand(CreateCategoryDto Dto, Guid ActorAdminId);
