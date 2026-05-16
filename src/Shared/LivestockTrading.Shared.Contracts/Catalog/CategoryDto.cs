namespace Shared.Contracts.Catalog;

using Shared.ValueObjects;

/// <summary>Kategori. ParentCode (3e Code-only; entity ParentId int → Infra resolve). Level: 1 = root, 2 = leaf (max depth, Karar 4c). Attributes v2 (boşsa empty list).</summary>
public sealed record CategoryDto(
    string Code,
    string? ParentCode,
    int Level,
    int DisplayOrder,
    bool IsActive,
    string? IconKey,
    Translations Name,
    Translations? Description,
    IReadOnlyList<CategoryAttributeDto> Attributes);
