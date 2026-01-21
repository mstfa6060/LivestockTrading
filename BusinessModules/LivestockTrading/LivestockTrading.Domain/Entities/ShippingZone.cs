using System;
using System.Collections.Generic;

namespace LivestockTrading.Domain.Entities;

/// <summary>
/// Kargo bölgeleri ve ücretleri
/// </summary>
public class ShippingZone : BaseEntity
{
    public Guid? SellerId { get; set; }
    public Seller Seller { get; set; }
    public string Name { get; set; }
    public string CountryCodes { get; set; }
    public bool IsActive { get; set; }
    public ICollection<ShippingRate> ShippingRates { get; set; }

    public ShippingZone()
    {
        ShippingRates = new HashSet<ShippingRate>();
        IsActive = true;
    }
}

/// <summary>
/// Kargo ücret kuralları
/// </summary>
public class ShippingRate : BaseEntity
{
    public Guid ShippingZoneId { get; set; }
    public ShippingZone ShippingZone { get; set; }
    public Guid? ShippingCarrierId { get; set; }
    public ShippingCarrier ShippingCarrier { get; set; }
    public decimal? MinWeight { get; set; }
    public decimal? MaxWeight { get; set; }
    public decimal? MinOrderAmount { get; set; }
    public decimal ShippingCost { get; set; }
    public string Currency { get; set; }
    public int? EstimatedDeliveryDays { get; set; }
    public bool IsFreeShipping { get; set; }
    public bool IsActive { get; set; }

    public ShippingRate()
    {
        Currency = "USD";
        IsFreeShipping = false;
        IsActive = true;
    }
}
