namespace Livestock.Features.Categories;

public record CategoryListItem(
    Guid Id,
    string Name,
    string Slug,
    string? Description,
    string? IconUrl,
    string? ImageUrl,
    int SortOrder,
    bool IsActive,
    Guid? ParentCategoryId,
    string? ParentCategoryName,
    int SubCategoryCount,
    DateTime CreatedAt);

public record CategoryDetail(
    Guid Id,
    string Name,
    string Slug,
    string? Description,
    string? IconUrl,
    string? ImageUrl,
    int SortOrder,
    bool IsActive,
    Guid? ParentCategoryId,
    string? ParentCategoryName,
    string? NameTranslations,
    string? DescriptionTranslations,
    DateTime CreatedAt);

public record CreateCategoryRequest(
    string Name,
    string Slug,
    string? Description,
    string? IconUrl,
    string? ImageUrl,
    Guid? ParentCategoryId,
    int SortOrder,
    bool IsActive);

public record UpdateCategoryRequest(
    Guid Id,
    string Name,
    string Slug,
    string? Description,
    string? IconUrl,
    string? ImageUrl,
    Guid? ParentCategoryId,
    int SortOrder,
    bool IsActive);

public record DeleteCategoryRequest(Guid Id);
public record GetCategoryRequest(Guid Id);
