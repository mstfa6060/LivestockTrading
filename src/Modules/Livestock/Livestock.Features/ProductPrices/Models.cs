namespace Livestock.Features.ProductPrices;

public record ProductPriceItem(
    Guid Id,
    Guid ProductId,
    string CurrencyCode,
    decimal Price,
    decimal? DiscountedPrice,
    string? CountryCodes,
    bool IsActive,
    bool IsAutomaticConversion,
    DateTime? ValidFrom,
    DateTime? ValidUntil,
    DateTime CreatedAt);

public record GetProductPricesRequest(Guid ProductId);
public record GetProductPriceRequest(Guid Id);

public record CreateProductPriceRequest(
    Guid ProductId,
    string CurrencyCode,
    decimal Price,
    decimal? DiscountedPrice,
    string? CountryCodes,
    bool IsActive = true,
    bool IsAutomaticConversion = false,
    DateTime? ValidFrom = null,
    DateTime? ValidUntil = null);

public record UpdateProductPriceRequest(
    Guid Id,
    string CurrencyCode,
    decimal Price,
    decimal? DiscountedPrice,
    string? CountryCodes,
    bool IsActive,
    bool IsAutomaticConversion,
    DateTime? ValidFrom,
    DateTime? ValidUntil);

public record DeleteProductPriceRequest(Guid Id);
