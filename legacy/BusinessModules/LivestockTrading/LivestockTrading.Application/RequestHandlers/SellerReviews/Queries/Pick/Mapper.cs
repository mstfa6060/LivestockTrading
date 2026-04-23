using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.SellerReviews.Queries.Pick;

public class Mapper
{
	public List<ResponseModel> MapToResponse(List<SellerReview> items)
	{
		return items.Select(e => new ResponseModel
		{
			Id = e.Id,
			Title = e.Title,
			OverallRating = e.OverallRating
		}).ToList();
	}
}
