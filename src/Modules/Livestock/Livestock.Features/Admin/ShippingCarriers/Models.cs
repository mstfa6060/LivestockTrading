namespace Livestock.Features.Admin.ShippingCarriers;

public record ShippingCarrierItem(
    Guid Id,
    string Name,
    string Code,
    string? Website,
    string? TrackingUrlTemplate,
    bool IsActive,
    string? SupportedCountries,
    DateTime CreatedAt);

public record GetShippingCarrierRequest(Guid Id);

public record CreateShippingCarrierRequest(
    string Name,
    string Code,
    string? Website,
    string? TrackingUrlTemplate,
    bool IsActive = true,
    string? SupportedCountries = null);

public record UpdateShippingCarrierRequest(
    Guid Id,
    string Name,
    string? Website,
    string? TrackingUrlTemplate,
    bool IsActive,
    string? SupportedCountries);

public record DeleteShippingCarrierRequest(Guid Id);
