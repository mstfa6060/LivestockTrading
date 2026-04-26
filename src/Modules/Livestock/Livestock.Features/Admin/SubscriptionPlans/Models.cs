using Livestock.Domain.Enums;

namespace Livestock.Features.Admin.SubscriptionPlans;

public record SubscriptionPlanAdminItem(
    Guid Id,
    string Name,
    string? Description,
    SubscriptionTier Tier,
    SubscriptionTargetType TargetType,
    SubscriptionPeriod Period,
    decimal Price,
    string CurrencyCode,
    string? AppStoreProductId,
    string? PlayStoreProductId,
    int MaxListings,
    int MaxPhotos,
    bool HasBoostDiscount,
    int? FreeBoostsPerMonth,
    bool IsActive,
    DateTime CreatedAt);

public record CreateSubscriptionPlanRequest(
    string Name,
    string? Description,
    SubscriptionTier Tier,
    SubscriptionTargetType TargetType,
    SubscriptionPeriod Period,
    decimal Price,
    string CurrencyCode,
    string? AppStoreProductId,
    string? PlayStoreProductId,
    int MaxListings,
    int MaxPhotos,
    bool HasBoostDiscount,
    int? FreeBoostsPerMonth);

public record UpdateSubscriptionPlanRequest(
    Guid Id,
    string Name,
    string? Description,
    decimal Price,
    string CurrencyCode,
    string? AppStoreProductId,
    string? PlayStoreProductId,
    int MaxListings,
    int MaxPhotos,
    bool HasBoostDiscount,
    int? FreeBoostsPerMonth,
    bool IsActive);

public record GetSubscriptionPlanAdminRequest(Guid Id);
public record DeleteSubscriptionPlanRequest(Guid Id);
