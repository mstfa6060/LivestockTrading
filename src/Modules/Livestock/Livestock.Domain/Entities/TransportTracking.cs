using Livestock.Domain.Enums;

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
    public DateTime OccurredAt { get; set; }

    public TransportRequest TransportRequest { get; set; } = null!;
}
