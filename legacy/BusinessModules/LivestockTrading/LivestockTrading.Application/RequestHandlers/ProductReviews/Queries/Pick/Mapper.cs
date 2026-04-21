using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.ProductReviews.Queries.Pick;

public class Mapper
{
	public List<ResponseModel> MapToResponse(List<ProductReview> items)
	{
		return items.Select(e => new ResponseModel
		{
			Id = e.Id,
			Title = e.Title,
			Rating = e.Rating
		}).ToList();
	}
}
