namespace Shared.Contracts.Catalog;

using Shared.ValueObjects;

/// <summary>Tam-nested ağaç düğümü (§8 tree pagination yok). Navigasyon-minimal; tam detay GetCategoryByCodeAsync. Level: 1 = root, 2 = leaf (Karar 4c).</summary>
public sealed record CategoryTreeDto(
    string Code,
    Translations Name,
    int Level,
    int DisplayOrder,
    bool IsActive,
    IReadOnlyList<CategoryTreeDto> Children);
