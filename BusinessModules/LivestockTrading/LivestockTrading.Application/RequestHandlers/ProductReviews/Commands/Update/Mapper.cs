using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.ProductReviews.Commands.Update;

public class Mapper
{
	public void MapToEntity(RequestModel request, ProductReview entity)
	{
		entity.ProductId = request.ProductId;
		entity.UserId = request.UserId;
		entity.OrderId = request.OrderId;
		entity.Rating = request.Rating;
		entity.Title = request.Title;
		entity.Comment = request.Comment;
		entity.IsVerifiedPurchase = request.IsVerifiedPurchase;
		entity.ImageUrls = request.ImageUrls;
		entity.VideoUrls = request.VideoUrls;
		entity.UpdatedAt = DateTime.UtcNow;
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
			UpdatedAt = entity.UpdatedAt
		};
	}
}
