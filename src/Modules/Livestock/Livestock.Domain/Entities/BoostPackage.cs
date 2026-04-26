using Livestock.Domain.Enums;

namespace Livestock.Domain.Entities;

public class BoostPackage : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public BoostType BoostType { get; set; }
    public int DurationDays { get; set; }
    public decimal Price { get; set; }
    public string CurrencyCode { get; set; } = "USD";
    public string? AppStoreProductId { get; set; }
    public string? PlayStoreProductId { get; set; }
    public int MultiplierFactor { get; set; } = 1;
    public bool IsActive { get; set; } = true;

    public ICollection<ProductBoost> ProductBoosts { get; set; } = [];
}
