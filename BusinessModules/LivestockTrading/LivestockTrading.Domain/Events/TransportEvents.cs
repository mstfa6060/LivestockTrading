namespace LivestockTrading.Domain.Events;

/// <summary>Yeni tasima talebi olusturuldu</summary>
public class TransportRequestCreatedEvent : IDomainEvent
{
    public Guid RequestId { get; set; }
    public Guid RequestUserId { get; set; }
    public string PickupCity { get; set; }
    public string DeliveryCity { get; set; }
    public string ProductTitle { get; set; }
    public DateTime OccurredAt { get; set; } = DateTime.UtcNow;
}

/// <summary>Nakliyeci teklif verdi</summary>
public class TransportOfferCreatedEvent : IDomainEvent
{
    public Guid OfferId { get; set; }
    public Guid RequestId { get; set; }
    public Guid TransporterUserId { get; set; }
    public Guid RequestOwnerUserId { get; set; }
    public string TransporterName { get; set; }
    public double OfferedPrice { get; set; }
    public DateTime OccurredAt { get; set; } = DateTime.UtcNow;
}

/// <summary>Teklif kabul/red edildi</summary>
public class TransportOfferStatusChangedEvent : IDomainEvent
{
    public Guid OfferId { get; set; }
    public Guid TransporterUserId { get; set; }
    public Guid RequestOwnerUserId { get; set; }
    public int NewStatus { get; set; }
    public DateTime OccurredAt { get; set; } = DateTime.UtcNow;
}
