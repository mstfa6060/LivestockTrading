namespace Livestock.Domain.Entities;

public class ShippingRate : BaseEntity
{
    public Guid ShippingZoneId { get; set; }
    public Guid? ShippingCarrierId { get; set; }
    public double? MinWeight { get; set; }
    public double? MaxWeight { get; set; }
    public decimal? MinOrderAmount { get; set; }
    public decimal ShippingCost { get; set; }
    public string CurrencyCode { get; set; } = "USD";
    public int? EstimatedDeliveryDays { get; set; }
    public bool IsFreeShipping { get; set; }
    public bool IsActive { get; set; } = true;

    public ShippingZone Zone { get; set; } = default!;
    public ShippingCarrier? Carrier { get; set; }
}
