namespace Livestock.Features.Admin.ShippingZones;

public record ShippingZoneItem(
    Guid Id,
    Guid? SellerId,
    string Name,
    string? CountryCodes,
    bool IsActive,
    DateTime CreatedAt);

public record GetShippingZoneRequest(Guid Id);

public record CreateShippingZoneRequest(
    string Name,
    Guid? SellerId,
    string? CountryCodes,
    bool IsActive = true);

public record UpdateShippingZoneRequest(
    Guid Id,
    string Name,
    Guid? SellerId,
    string? CountryCodes,
    bool IsActive);

public record DeleteShippingZoneRequest(Guid Id);
