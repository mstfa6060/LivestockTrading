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
    DateTime CreatedAt);

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
    bool IsNegotiable);

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
