using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.ProductReviews.Queries.All;

public class Mapper
{
	public List<ResponseModel> MapToResponse(List<ProductReview> items)
	{
		return items.Select(e => new ResponseModel
		{
			Id = e.Id,
			ProductId = e.ProductId,
			UserId = e.UserId,
			Rating = e.Rating,
			Title = e.Title,
			Comment = e.Comment,
			IsVerifiedPurchase = e.IsVerifiedPurchase,
			IsApproved = e.IsApproved,
			HelpfulCount = e.HelpfulCount,
			CreatedAt = e.CreatedAt
		}).ToList();
	}
}
