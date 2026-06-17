using Livestock.Domain.Enums;

namespace Livestock.Domain.Entities;

public class TransportOffer : BaseEntity
{
    public Guid TransportRequestId { get; set; }
    public Guid TransporterId { get; set; }
    public decimal Price { get; set; }
    public string CurrencyCode { get; set; } = "USD";
    public string? Note { get; set; }
    public int? EstimatedDaysMin { get; set; }
    public int? EstimatedDaysMax { get; set; }
    public TransportOfferStatus Status { get; set; } = TransportOfferStatus.Pending;
    public DateTime? ExpiresAt { get; set; }

    public TransportRequest TransportRequest { get; set; } = null!;
    public Transporter Transporter { get; set; } = null!;
}
