using Livestock.Domain.Enums;
using NetTopologySuite.Geometries;

namespace Livestock.Domain.Entities;

public class TransportTracking : BaseEntity
{
    public Guid TransportRequestId { get; set; }
    public Guid TransporterId { get; set; }
    public TrackingStatus Status { get; set; }
    public string? Note { get; set; }
    public string? Location { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }

    /// <summary>
    /// PostGIS geography(Point, 4326). Populated alongside Latitude/Longitude;
    /// enables GIST-indexed proximity queries for live tracking. Legacy lat/lng
    /// fields remain for backward-compat.
    /// </summary>
    public Point? Geo { get; set; }

    public DateTime OccurredAt { get; set; }

    public TransportRequest TransportRequest { get; set; } = null!;
}
