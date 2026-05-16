namespace Shared.Contracts.Catalog.Admin;

using Shared.ValueObjects;

/// <summary>Brand.CreateByAdmin. CategoryCodes (Code-only set); Id Guid server (3e istisnası, dış-güvenli).</summary>
public sealed record CreateBrandDto(
    string Slug,
    Translations Name,
    Translations? Description,
    IReadOnlyList<string> CategoryCodes,
    string? LogoUrl,
    string? Website,
    string? OriginCountryCode,
    int DisplayOrder);
