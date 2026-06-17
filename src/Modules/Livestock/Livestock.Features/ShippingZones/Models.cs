namespace Livestock.Features.ShippingZones;

public record ShippingZoneItem(
    Guid Id,
    Guid? SellerId,
    string Name,
    string CountryCodes,
    bool IsActive,
    DateTime CreatedAt);

public record GetZonesRequest(Guid? SellerId);
public record GetZoneRequest(Guid Id);

public record CreateZoneRequest(
    Guid? SellerId,
    string Name,
    string CountryCodes,
    bool IsActive = true);

public record UpdateZoneRequest(
    Guid Id,
    Guid? SellerId,
    string Name,
    string CountryCodes,
    bool IsActive);

public record DeleteZoneRequest(Guid Id);
