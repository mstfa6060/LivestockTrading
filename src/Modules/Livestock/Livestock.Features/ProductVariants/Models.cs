namespace Livestock.Features.ProductVariants;

public record ProductVariantDetail(
    Guid Id, Guid ProductId,
    string Name, string? Sku,
    decimal PriceAdjustment, int Quantity,
    string? Attributes, bool IsActive, DateTime CreatedAt);

public record CreateProductVariantRequest(
    Guid ProductId,
    string Name, string? Sku,
    decimal PriceAdjustment, int Quantity,
    string? Attributes, bool IsActive);

public record UpdateProductVariantRequest(
    Guid Id,
    string Name, string? Sku,
    decimal PriceAdjustment, int Quantity,
    string? Attributes, bool IsActive);

public record GetProductVariantsRequest(Guid ProductId);
public record GetProductVariantRequest(Guid Id);
public record DeleteProductVariantRequest(Guid Id);
