using Livestock.Domain.Enums;

namespace Livestock.Domain.Entities;

public class ProductBoost : BaseEntity
{
    public Guid ProductId { get; set; }
    public Guid SellerId { get; set; }
    public Guid PackageId { get; set; }
    public BoostType BoostType { get; set; }
    public DateTime StartsAt { get; set; }
    public DateTime EndsAt { get; set; }
    public bool IsActive { get; set; } = true;
    public decimal PaidAmount { get; set; }
    public string CurrencyCode { get; set; } = "USD";

    public Product Product { get; set; } = null!;
    public BoostPackage Package { get; set; } = null!;
}
