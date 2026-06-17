using Livestock.Domain.Enums;
using NetTopologySuite.Geometries;

namespace Livestock.Domain.Entities;

public class Location : BaseEntity
{
    public string CountryCode { get; set; } = string.Empty;
    public string? CountryName { get; set; }
    public string? State { get; set; }
    public string? City { get; set; }
    public string? District { get; set; }
    public string? PostalCode { get; set; }
    public string? AddressLine { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public LocationType LocationType { get; set; }

    /// <summary>
    /// PostGIS geography(Point, 4326). Populated alongside Latitude/Longitude;
    /// enables GIST-indexed ST_DWithin / ST_Distance queries. Latitude/Longitude
    /// are preserved for backward-compat and will be dropped two releases later.
    /// </summary>
    public Point? Geo { get; set; }

    public Guid OwnerId { get; set; }
    public string OwnerType { get; set; } = string.Empty;
}
