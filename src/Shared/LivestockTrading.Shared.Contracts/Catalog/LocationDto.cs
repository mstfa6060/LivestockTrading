namespace Shared.Contracts.Catalog;

using Shared.ValueObjects;

/// <summary>Lokasyon. Id = int DIŞA AÇIK (3e madde 1 istisnası, frontend /locations/{id}). Centroid YOK (Faz 1, NTS C3). CountryCode plain ISO string.</summary>
public sealed record LocationDto(
    int Id,
    int? ParentId,
    LocationLevel Level,
    string CountryCode,
    string Code,
    string Slug,
    string Path,
    Translations Name,
    Translations? NativeName,
    int Population,
    bool IsActive,
    int DisplayOrder);
