namespace LivestockTrading.Catalog.Domain.Entities;

using NetTopologySuite.Geometries;
using Shared.Domain;
using Shared.ValueObjects;

/// <summary>
/// Location reference (v2 — 5-level hierarchy). Entity türevi, AR DEĞİL. Doc 05-catalog §4
/// birebir. INT PK (Karar 3e istisnası — slug çakışma). Centroid Faz 1 dahil (NTS, Sapma 35).
/// LocationLevel enum dosya-içi tanım (Catalog.Domain → Shared.Contracts ref YOK, S1=(i)).
/// Faz 1'de behavior yok (doc §4 method göstermiyor).
/// </summary>
public sealed class Location : Entity
{
    public int Id { get; private set; }                         // INT PK (Karar 3e istisnası — slug çakışma)
    public int? ParentId { get; private set; }                  // self-ref
    public LocationLevel Level { get; private set; }
    public CountryCode CountryCode { get; private set; }        // denormalize: tree root'tan kopya

    public string Code { get; private set; }                    // TR: il plaka (34), ilçe ID
    public string Slug { get; private set; }                    // kebab-case
    public string Path { get; private set; }                    // "tr/marmara/istanbul/kadikoy/caferaga"
    public Translations Name { get; private set; }
    public Translations? NativeName { get; private set; }       // local script

    public Point? Centroid { get; private set; }                // PostGIS Point Faz 1
    public string? PolygonGeoJsonUrl { get; private set; }      // Faz 2 MinIO MultiPolygon

    public bool IsActive { get; private set; }
    public int Population { get; private set; }
    public int DisplayOrder { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }

    public Location(
        int? parentId,
        LocationLevel level,
        CountryCode countryCode,
        string code,
        string slug,
        string path,
        Translations name,
        Translations? nativeName,
        Point? centroid,
        int population,
        int displayOrder)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new DomainException("Location code is required.");
        if (string.IsNullOrWhiteSpace(slug))
            throw new DomainException("Location slug is required.");
        if (string.IsNullOrWhiteSpace(path))
            throw new DomainException("Location path is required.");

        ParentId = parentId;
        Level = level;
        CountryCode = countryCode;
        Code = code;
        Slug = slug;
        Path = path;
        Name = name;
        NativeName = nativeName;
        Centroid = centroid;
        Population = population;
        DisplayOrder = displayOrder;
        IsActive = true;
        CreatedAt = DateTimeOffset.UtcNow;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    protected override object IdentityValue => Id;
    protected override bool IsTransient => Id == 0;
}

public enum LocationLevel
{
    Country = 1,
    Region = 2,         // TR: Marmara, Ege; bazı ülkelerde skip
    State = 3,          // TR: İl (81); US: State (50)
    District = 4,       // TR: İlçe (~970)
    Neighborhood = 5    // TR: Mahalle/Köy (~50,000+)
}
