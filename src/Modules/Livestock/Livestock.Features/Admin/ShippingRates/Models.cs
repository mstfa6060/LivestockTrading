namespace Livestock.Features.Admin.ShippingRates;

public record ShippingRateItem(
    Guid Id,
    Guid ShippingZoneId,
    Guid? ShippingCarrierId,
    double? MinWeight,
    double? MaxWeight,
    double? MinOrderAmount,
    decimal ShippingCost,
    string Currency,
    int? EstimatedDeliveryDays,
    bool IsFreeShipping,
    bool IsActive,
    DateTime CreatedAt);

public record GetShippingRatesByZoneRequest(Guid ShippingZoneId);
public record GetShippingRateRequest(Guid Id);

public record CreateShippingRateRequest(
    Guid ShippingZoneId,
    Guid? ShippingCarrierId,
    double? MinWeight,
    double? MaxWeight,
    double? MinOrderAmount,
    decimal ShippingCost,
    string Currency = "USD",
    int? EstimatedDeliveryDays = null,
    bool IsFreeShipping = false,
    bool IsActive = true);

public record UpdateShippingRateRequest(
    Guid Id,
    Guid? ShippingCarrierId,
    double? MinWeight,
    double? MaxWeight,
    double? MinOrderAmount,
    decimal ShippingCost,
    string Currency,
    int? EstimatedDeliveryDays,
    bool IsFreeShipping,
    bool IsActive);

public record DeleteShippingRateRequest(Guid Id);
