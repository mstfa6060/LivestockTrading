namespace Livestock.Domain.Entities;

public class ProductVariant : BaseEntity
{
    public Guid ProductId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Sku { get; set; }
    public decimal PriceAdjustment { get; set; }
    public int Quantity { get; set; }
    public string? Attributes { get; set; }
    public bool IsActive { get; set; } = true;

    public Product Product { get; set; } = null!;
}
