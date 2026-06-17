namespace Livestock.Features.Categories.Update;

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
