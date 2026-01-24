using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.SellerReviews.Commands.Update;

public class Mapper
{
	public void MapToEntity(RequestModel request, SellerReview entity)
	{
		entity.SellerId = request.SellerId;
		entity.UserId = request.UserId;
		entity.OrderId = request.OrderId;
		entity.OverallRating = request.OverallRating;
		entity.CommunicationRating = request.CommunicationRating;
		entity.ShippingSpeedRating = request.ShippingSpeedRating;
		entity.ProductQualityRating = request.ProductQualityRating;
		entity.Title = request.Title;
		entity.Comment = request.Comment;
		entity.IsVerifiedPurchase = request.IsVerifiedPurchase;
		entity.UpdatedAt = DateTime.UtcNow;
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
			UpdatedAt = entity.UpdatedAt
		};
	}
}
