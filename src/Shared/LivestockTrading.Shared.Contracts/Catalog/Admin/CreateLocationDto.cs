namespace Shared.Contracts.Catalog.Admin;

using Shared.ValueObjects;
using Shared.Contracts.Catalog;

/// <summary>Slug/Path server-gen (SlugHelper + parent zinciri, DTO'da YOK). ParentId int? (Location 3e int-id istisnası, parent-by-int).</summary>
public sealed record CreateLocationDto(
    int? ParentId,
    LocationLevel Level,
    string CountryCode,
    string Code,
    Translations Name,
    Translations? NativeName,
    int Population,
    int DisplayOrder);
