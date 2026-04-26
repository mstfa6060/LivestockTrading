using Livestock.Domain.Enums;

namespace Livestock.Features.Admin.Products;

public record PendingProductItem(
    Guid Id,
    string Title,
    string Slug,
    decimal Price,
    string CurrencyCode,
    Guid SellerId,
    string SellerBusinessName,
    Guid CategoryId,
    string CategoryName,
    DateTime CreatedAt);
