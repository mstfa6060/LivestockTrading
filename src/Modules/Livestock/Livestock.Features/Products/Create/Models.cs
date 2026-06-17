using Livestock.Domain.Enums;

namespace Livestock.Features.Products.Create;

// Handler normalizes legacy aliases from the old frontend client:
//   BasePrice -> Price, Currency -> CurrencyCode, StockQuantity -> Quantity,
//   StockUnit -> Unit, MediaBucketId -> BucketId. LocationId/ShortDescription
//   persist to the Product entity; the rest of the noise (metaTitle, status,
//   shippingCost, attributes, etc.) is accepted and dropped.
public record CreateProductRequest(
    Guid CategoryId,
    Guid? BrandId,
    Guid? FarmId,
    string Title,
    string Slug,
    string? Description,
    decimal Price,
    string CurrencyCode,
    int Quantity,
    string? Unit,
    ProductCondition Condition,
    bool IsNegotiable,
    // Legacy-compat fields sent by the generated frontend client:
    decimal? BasePrice,
    string? Currency,
    int? StockQuantity,
    string? StockUnit,
    string? ShortDescription,
    Guid? LocationId,
    Guid? MediaBucketId,
    string? PriceUnit,
    decimal? DiscountedPrice,
    int? MinOrderQuantity,
    int? MaxOrderQuantity,
    bool? IsShippingAvailable,
    decimal? ShippingCost,
    bool? IsInternationalShipping,
    decimal? Weight,
    string? WeightUnit,
    string? Attributes,
    string? MetaTitle,
    string? MetaDescription,
    string? MetaKeywords,
    Guid? CoverImageFileId,
    Guid? SellerId);
