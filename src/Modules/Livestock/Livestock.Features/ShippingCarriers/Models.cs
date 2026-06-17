namespace Livestock.Features.ShippingCarriers;

public record ShippingCarrierItem(
    Guid Id,
    string Name,
    string Code,
    string? Website,
    string? TrackingUrlTemplate,
    bool IsActive,
    string? SupportedCountries,
    DateTime CreatedAt);

public record GetCarrierRequest(Guid Id);

public record CreateCarrierRequest(
    string Name,
    string Code,
    string? Website,
    string? TrackingUrlTemplate,
    bool IsActive = true,
    string? SupportedCountries = null);

public record UpdateCarrierRequest(
    Guid Id,
    string Name,
    string Code,
    string? Website,
    string? TrackingUrlTemplate,
    bool IsActive,
    string? SupportedCountries);

public record DeleteCarrierRequest(Guid Id);
