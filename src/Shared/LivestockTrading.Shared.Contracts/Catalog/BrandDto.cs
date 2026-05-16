namespace Shared.Contracts.Catalog;

using Shared.ValueObjects;

/// <summary>Marka. Id = Guid (3e istisnası, dış-güvenli). CategoryCodes BrandCategory junction'dan (Code-only). Status = workflow state (IsActive soft-delete'ten ayrı).</summary>
public sealed record BrandDto(
    Guid Id,
    string Slug,
    Translations Name,
    Translations? Description,
    string? LogoUrl,
    string? Website,
    string? OriginCountryCode,
    BrandStatus Status,
    bool IsActive,
    int DisplayOrder,
    IReadOnlyList<string> CategoryCodes);
