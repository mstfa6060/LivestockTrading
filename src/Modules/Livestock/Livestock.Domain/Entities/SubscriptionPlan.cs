using Livestock.Domain.Enums;

namespace Livestock.Domain.Entities;

public class SubscriptionPlan : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public SubscriptionTier Tier { get; set; }
    public SubscriptionTargetType TargetType { get; set; }
    public SubscriptionPeriod Period { get; set; }
    public decimal Price { get; set; }
    public string CurrencyCode { get; set; } = "USD";
    public string? AppStoreProductId { get; set; }
    public string? PlayStoreProductId { get; set; }
    public int MaxListings { get; set; }
    public int MaxPhotos { get; set; }
    public bool HasBoostDiscount { get; set; }
    public int? FreeBoostsPerMonth { get; set; }
    public bool IsActive { get; set; } = true;

    public ICollection<SellerSubscription> Subscriptions { get; set; } = [];
}
