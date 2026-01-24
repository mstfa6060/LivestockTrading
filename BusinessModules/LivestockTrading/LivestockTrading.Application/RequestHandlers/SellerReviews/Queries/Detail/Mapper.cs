using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.SellerReviews.Queries.Detail;

public class Mapper
{
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
			IsApproved = entity.IsApproved,
			ApprovedAt = entity.ApprovedAt,
			HelpfulCount = entity.HelpfulCount,
			NotHelpfulCount = entity.NotHelpfulCount,
			SellerResponse = entity.SellerResponse,
			SellerRespondedAt = entity.SellerRespondedAt,
			CreatedAt = entity.CreatedAt,
			UpdatedAt = entity.UpdatedAt
		};
	}
}
