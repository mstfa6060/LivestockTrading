using Livestock.Domain.Enums;

namespace Livestock.Features.Products.Update;

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
