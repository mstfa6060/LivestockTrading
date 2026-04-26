namespace Livestock.Features.Categories.Create;

public record CreateCategoryRequest(
    string Name,
    string Slug,
    string? Description,
    string? IconUrl,
    string? ImageUrl,
    Guid? ParentCategoryId,
    int SortOrder,
    bool IsActive);
