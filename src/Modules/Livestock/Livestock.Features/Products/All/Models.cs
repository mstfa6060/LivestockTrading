using Livestock.Domain.Enums;

namespace Livestock.Features.Products.All;

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
    DateTime CreatedAt,
    // Legacy aliases for the frontend my-listings/product-card consumers:
    decimal BasePrice,
    string Currency,
    int StockQuantity,
    bool IsInStock,
    string? ShortDescription,
    Guid? LocationId,
    string? LocationCountryCode,
    string? LocationCity,
    decimal? DiscountedPrice,
    Guid? MediaBucketId);

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
    int PageSize = 20,
    // Legacy frontend shape: flat SellerId, plus a generic query-builder
    // payload `{filters:[{key,values,...}], sorting, pageRequest}`. Handler
    // reads SellerId top-level first, falls back to the filters array.
    Guid? SellerId = null,
    List<ProductFilterItem>? Filters = null,
    ProductPageRequest? PageRequest = null,
    string? Slug = null);

public record ProductFilterItem(
    string? Key,
    string? Type,
    bool? IsUsed,
    List<object>? Values,
    string? ConditionType);

public record ProductPageRequest(
    int? CurrentPage,
    int? PerPageCount,
    bool? ListAll);
