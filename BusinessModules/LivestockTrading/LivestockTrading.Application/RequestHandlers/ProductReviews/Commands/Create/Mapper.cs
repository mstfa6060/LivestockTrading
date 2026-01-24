using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.ProductReviews.Commands.Create;

public class Mapper
{
	public ProductReview MapToEntity(RequestModel request)
	{
		return new ProductReview
		{
			Id = Guid.NewGuid(),
			ProductId = request.ProductId,
			UserId = request.UserId,
			OrderId = request.OrderId,
			Rating = request.Rating,
			Title = request.Title,
			Comment = request.Comment,
			IsVerifiedPurchase = request.IsVerifiedPurchase,
			ImageUrls = request.ImageUrls,
			VideoUrls = request.VideoUrls,
			CreatedAt = DateTime.UtcNow
		};
	}

	public ResponseModel MapToResponse(ProductReview entity)
	{
		return new ResponseModel
		{
			Id = entity.Id,
			ProductId = entity.ProductId,
			UserId = entity.UserId,
			OrderId = entity.OrderId,
			Rating = entity.Rating,
			Title = entity.Title,
			Comment = entity.Comment,
			IsVerifiedPurchase = entity.IsVerifiedPurchase,
			ImageUrls = entity.ImageUrls,
			VideoUrls = entity.VideoUrls,
			CreatedAt = entity.CreatedAt
		};
	}
}
