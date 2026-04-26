using Livestock.Domain.Enums;

namespace Livestock.Features.Subscriptions;

public record SubscriptionPlanItem(Guid Id, string Name, string? Description, SubscriptionTier Tier, SubscriptionTargetType TargetType, SubscriptionPeriod Period, decimal Price, string CurrencyCode, int MaxListings, int MaxPhotos, bool HasBoostDiscount, int? FreeBoostsPerMonth, bool IsActive);
public record MySubscriptionItem(Guid Id, Guid PlanId, string PlanName, SubscriptionTier Tier, SubscriptionStatus Status, SubscriptionPeriod Period, DateTime StartedAt, DateTime ExpiresAt, decimal PaidAmount, string CurrencyCode);
public record BoostPackageItem(Guid Id, string Name, string? Description, BoostType BoostType, int DurationDays, decimal Price, string CurrencyCode, bool IsActive);

public record GetSubscriptionPlansRequest(SubscriptionTargetType TargetType);
public record SubscribeRequest(
    Guid PlanId,
    SubscriptionPlatform Platform,
    string Receipt,
    string StoreTransactionId,
    string? ExternalSubscriptionId);
public record PurchaseBoostRequest(Guid ProductId, Guid PackageId, SubscriptionPlatform Platform);
