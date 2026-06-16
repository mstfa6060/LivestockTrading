using Livestock.Domain.Enums;

namespace Livestock.Domain.Entities;

public class TransportRequest : BaseEntity
{
    public Guid RequesterUserId { get; set; }
    public Guid? SellerId { get; set; }
    public Guid? ProductId { get; set; }
    public Guid? AssignedTransporterId { get; set; }

    public string PickupCountryCode { get; set; } = string.Empty;
    public string PickupCity { get; set; } = string.Empty;
    public string? PickupAddress { get; set; }
    public double? PickupLatitude { get; set; }
    public double? PickupLongitude { get; set; }

    public string DeliveryCountryCode { get; set; } = string.Empty;
    public string DeliveryCity { get; set; } = string.Empty;
    public string? DeliveryAddress { get; set; }
    public double? DeliveryLatitude { get; set; }
    public double? DeliveryLongitude { get; set; }

    public TransportType TransportType { get; set; }
    public TransportRequestStatus Status { get; set; } = TransportRequestStatus.Pending;
    public string? CargoDescription { get; set; }
    public int? AnimalCount { get; set; }
    public decimal? EstimatedWeightKg { get; set; }
    public DateTime? PickupDate { get; set; }
    public string? SpecialRequirements { get; set; }
    public decimal? Budget { get; set; }
    public string? CurrencyCode { get; set; }
    public DateTime? PoolExpiresAt { get; set; }

    public Transporter? AssignedTransporter { get; set; }
    public ICollection<TransportOffer> Offers { get; set; } = [];
    public ICollection<TransportTracking> TrackingUpdates { get; set; } = [];
}
