using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.SellerReviews.Queries.All;

public class Mapper
{
	public List<ResponseModel> MapToResponse(List<SellerReview> items)
	{
		return items.Select(e => new ResponseModel
		{
			Id = e.Id,
			SellerId = e.SellerId,
			UserId = e.UserId,
			OverallRating = e.OverallRating,
			CommunicationRating = e.CommunicationRating,
			ShippingSpeedRating = e.ShippingSpeedRating,
			ProductQualityRating = e.ProductQualityRating,
			Title = e.Title,
			IsVerifiedPurchase = e.IsVerifiedPurchase,
			IsApproved = e.IsApproved,
			CreatedAt = e.CreatedAt
		}).ToList();
	}
}
