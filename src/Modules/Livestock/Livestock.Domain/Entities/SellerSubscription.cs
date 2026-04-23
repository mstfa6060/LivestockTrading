using Livestock.Domain.Enums;

namespace Livestock.Domain.Entities;

public class SellerSubscription : BaseEntity
{
    public Guid SubscriberId { get; set; }
    public SubscriptionTargetType SubscriberType { get; set; }
    public Guid PlanId { get; set; }
    public SubscriptionStatus Status { get; set; } = SubscriptionStatus.Active;
    public SubscriptionPeriod Period { get; set; }
    public SubscriptionPlatform Platform { get; set; }
    public string? ExternalSubscriptionId { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
    public DateTime? CancelledAt { get; set; }
    public decimal PaidAmount { get; set; }
    public string CurrencyCode { get; set; } = "USD";

    public SubscriptionPlan Plan { get; set; } = null!;
}
