namespace Livestock.Domain.Entities;

public class ShippingRate : BaseEntity
{
    public Guid ShippingZoneId { get; set; }
    public Guid? ShippingCarrierId { get; set; }
    public double? MinWeight { get; set; }
    public double? MaxWeight { get; set; }
    public double? MinOrderAmount { get; set; }
    public decimal ShippingCost { get; set; }
    public string Currency { get; set; } = "USD";
    public int? EstimatedDeliveryDays { get; set; }
    public bool IsFreeShipping { get; set; }
    public bool IsActive { get; set; } = true;

    public ShippingZone ShippingZone { get; set; } = null!;
    public ShippingCarrier? ShippingCarrier { get; set; }
}
