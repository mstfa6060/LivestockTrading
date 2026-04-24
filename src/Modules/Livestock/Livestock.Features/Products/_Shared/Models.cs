using Livestock.Domain.Enums;

namespace Livestock.Features.Products;

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

public record PublishProductRequest(Guid Id);
