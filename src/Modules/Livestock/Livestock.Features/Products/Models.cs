using Livestock.Domain.Enums;

namespace Livestock.Features.Products;

public record ProductListItem(
    Guid Id,
    string Title,
    string Slug,
    decimal Price,
    string CurrencyCode,
    int Quantity,
    string? Unit,
    ProductStatus Status,
    ProductCondition Condition,
    bool IsNegotiable,
    bool IsFeatured,
    Guid SellerId,
    string SellerBusinessName,
    Guid CategoryId,
    string CategoryName,
    Guid? BrandId,
    string? BrandName,
    string? CountryCode,
    string? City,
    double AverageRating,
    int ReviewCount,
    int ViewCount,
    DateTime CreatedAt);

public record ProductDetail(
    Guid Id,
    string Title,
    string Slug,
    string? Description,
    decimal Price,
    string CurrencyCode,
    int Quantity,
    string? Unit,
    ProductStatus Status,
    ProductCondition Condition,
    bool IsNegotiable,
    bool IsFeatured,
    Guid SellerId,
    string SellerBusinessName,
    Guid CategoryId,
    string CategoryName,
    Guid? BrandId,
    string? BrandName,
    Guid? FarmId,
    string? FarmName,
    Guid? LocationId,
    string? CountryCode,
    string? City,
    double? Latitude,
    double? Longitude,
    double AverageRating,
    int ReviewCount,
    int ViewCount,
    DateTime? PublishedAt,
    DateTime CreatedAt,
    // Frontend reads `mediaBucketId` to query the Files bucket for the
    // product's images; without it the detail page shows no gallery.
    Guid? MediaBucketId);

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

public record UpdateProductRequest(
    Guid Id,
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
    bool IsNegotiable);

public record DeleteProductRequest(Guid Id);
public record GetProductRequest(Guid Id);
public record ApproveProductRequest(Guid Id);
public record RejectProductRequest(Guid Id, string Reason);
public record PublishProductRequest(Guid Id);

public record ProductSearchRequest(
    string? Keyword,
    Guid? CategoryId,
    Guid? BrandId,
    string? CountryCode,
    decimal? MinPrice,
    decimal? MaxPrice,
    bool? IsNegotiable,
    ProductCondition? Condition,
    int Page = 1,
    int PageSize = 20);
