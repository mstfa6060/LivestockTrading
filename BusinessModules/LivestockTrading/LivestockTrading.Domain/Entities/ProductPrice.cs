using System;

namespace LivestockTrading.Domain.Entities;

/// <summary>
/// Ürün için farklı para birimlerinde fiyatlandırma
/// </summary>
public class ProductPrice : BaseEntity
{
    public Guid ProductId { get; set; }
    public Product Product { get; set; }
    public string CurrencyCode { get; set; }
    public decimal Price { get; set; }
    public decimal? DiscountedPrice { get; set; }
    public string CountryCodes { get; set; }
    public bool IsActive { get; set; }
    public DateTime? ValidFrom { get; set; }
    public DateTime? ValidUntil { get; set; }
    public bool IsAutomaticConversion { get; set; }

    public ProductPrice()
    {
        IsActive = true;
        IsAutomaticConversion = false;
    }
}
