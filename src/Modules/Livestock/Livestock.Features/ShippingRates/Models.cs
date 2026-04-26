namespace Livestock.Features.ShippingRates;

public record ShippingRateItem(
    Guid Id,
    Guid ShippingZoneId,
    Guid? ShippingCarrierId,
    double? MinWeight,
    double? MaxWeight,
    decimal? MinOrderAmount,
    decimal ShippingCost,
    string CurrencyCode,
    int? EstimatedDeliveryDays,
    bool IsFreeShipping,
    bool IsActive,
    DateTime CreatedAt);

public record GetRatesRequest(Guid ShippingZoneId);
public record GetRateRequest(Guid Id);

public record CreateRateRequest(
    Guid ShippingZoneId,
    Guid? ShippingCarrierId,
    double? MinWeight,
    double? MaxWeight,
    decimal? MinOrderAmount,
    decimal ShippingCost,
    string CurrencyCode,
    int? EstimatedDeliveryDays,
    bool IsFreeShipping = false,
    bool IsActive = true);

public record UpdateRateRequest(
    Guid Id,
    Guid ShippingZoneId,
    Guid? ShippingCarrierId,
    double? MinWeight,
    double? MaxWeight,
    decimal? MinOrderAmount,
    decimal ShippingCost,
    string CurrencyCode,
    int? EstimatedDeliveryDays,
    bool IsFreeShipping,
    bool IsActive);

public record DeleteRateRequest(Guid Id);
