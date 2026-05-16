namespace Shared.Contracts.Catalog.Admin;

using Shared.ValueObjects;

/// <summary>Category.CreateTopLevel/Subcategory. Level server-derive (ParentCode null→1 else 2); Id/IsActive server.</summary>
public sealed record CreateCategoryDto(
    string Code,
    string? ParentCode,
    Translations Name,
    Translations? Description,
    int DisplayOrder,
    string? IconKey);
