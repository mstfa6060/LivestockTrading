namespace Livestock.Domain.Entities;

public class ProductPrice : BaseEntity
{
    public Guid ProductId { get; set; }
    public string CurrencyCode { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public decimal? DiscountedPrice { get; set; }
    public string? CountryCodes { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsAutomaticConversion { get; set; }
    public DateTime? ValidFrom { get; set; }
    public DateTime? ValidUntil { get; set; }

    public Product Product { get; set; } = null!;
}
