namespace Livestock.Features.ProductImages;

public record ProductImageItem(
    Guid Id,
    Guid ProductId,
    string ImageUrl,
    string? ThumbnailUrl,
    string? AltText,
    int SortOrder,
    bool IsPrimary,
    DateTime CreatedAt);

public record GetProductImagesRequest(Guid ProductId);
public record GetProductImageRequest(Guid Id);

public record CreateProductImageRequest(
    Guid ProductId,
    string ImageUrl,
    string? ThumbnailUrl,
    string? AltText,
    int SortOrder = 0,
    bool IsPrimary = false);

public record DeleteProductImageRequest(Guid Id);
public record SetPrimaryImageRequest(Guid Id);
