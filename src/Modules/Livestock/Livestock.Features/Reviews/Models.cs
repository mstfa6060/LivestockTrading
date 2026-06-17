namespace Livestock.Features.Reviews;

public record ProductReviewItem(Guid Id, Guid ProductId, Guid ReviewerUserId, int Rating, string? Comment, bool IsVerifiedPurchase, DateTime CreatedAt);
public record SellerReviewItem(Guid Id, Guid SellerId, Guid ReviewerUserId, int Rating, string? Comment, DateTime CreatedAt);
public record TransporterReviewItem(Guid Id, Guid TransporterId, Guid ReviewerUserId, int Rating, string? Comment, DateTime CreatedAt);

public record CreateProductReviewRequest(Guid ProductId, int Rating, string? Comment);
public record CreateSellerReviewRequest(Guid SellerId, int Rating, string? Comment, Guid? DealId);
public record CreateTransporterReviewRequest(Guid TransporterId, int Rating, string? Comment, Guid? TransportRequestId);
public record GetProductReviewsRequest(Guid ProductId, int Page = 1, int PageSize = 20);
public record GetSellerReviewsRequest(Guid SellerId, int Page = 1, int PageSize = 20);
public record GetTransporterReviewsRequest(Guid TransporterId, int Page = 1, int PageSize = 20);

public record UpdateProductReviewRequest(Guid Id, int Rating, string? Comment);
public record UpdateSellerReviewRequest(Guid Id, int Rating, string? Comment);
public record UpdateTransporterReviewRequest(Guid Id, int Rating, string? Comment);

public record DeleteReviewRequest(Guid Id);
