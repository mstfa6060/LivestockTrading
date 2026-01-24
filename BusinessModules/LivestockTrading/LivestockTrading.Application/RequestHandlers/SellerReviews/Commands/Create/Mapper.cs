using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.SellerReviews.Commands.Create;

public class Mapper
{
	public SellerReview MapToEntity(RequestModel request)
	{
		return new SellerReview
		{
			Id = Guid.NewGuid(),
			SellerId = request.SellerId,
			UserId = request.UserId,
			OrderId = request.OrderId,
			OverallRating = request.OverallRating,
			CommunicationRating = request.CommunicationRating,
			ShippingSpeedRating = request.ShippingSpeedRating,
			ProductQualityRating = request.ProductQualityRating,
			Title = request.Title,
			Comment = request.Comment,
			IsVerifiedPurchase = request.IsVerifiedPurchase,
			CreatedAt = DateTime.UtcNow
		};
	}

	public ResponseModel MapToResponse(SellerReview entity)
	{
		return new ResponseModel
		{
			Id = entity.Id,
			SellerId = entity.SellerId,
			UserId = entity.UserId,
			OrderId = entity.OrderId,
			OverallRating = entity.OverallRating,
			CommunicationRating = entity.CommunicationRating,
			ShippingSpeedRating = entity.ShippingSpeedRating,
			ProductQualityRating = entity.ProductQualityRating,
			Title = entity.Title,
			Comment = entity.Comment,
			IsVerifiedPurchase = entity.IsVerifiedPurchase,
			CreatedAt = entity.CreatedAt
		};
	}
}
