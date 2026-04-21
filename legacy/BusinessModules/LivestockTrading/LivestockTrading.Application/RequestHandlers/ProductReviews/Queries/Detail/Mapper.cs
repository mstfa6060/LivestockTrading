using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.ProductReviews.Queries.Detail;

public class Mapper
{
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
			IsApproved = entity.IsApproved,
			ApprovedAt = entity.ApprovedAt,
			ApprovedByUserId = entity.ApprovedByUserId,
			HelpfulCount = entity.HelpfulCount,
			NotHelpfulCount = entity.NotHelpfulCount,
			SellerResponse = entity.SellerResponse,
			SellerRespondedAt = entity.SellerRespondedAt,
			ImageUrls = entity.ImageUrls,
			VideoUrls = entity.VideoUrls,
			CreatedAt = entity.CreatedAt,
			UpdatedAt = entity.UpdatedAt
		};
	}
}
