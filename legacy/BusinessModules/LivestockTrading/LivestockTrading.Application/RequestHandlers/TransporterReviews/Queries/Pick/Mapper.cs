using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.TransporterReviews.Queries.Pick;

public class Mapper
{
	public List<ResponseModel> MapToResponse(List<TransporterReview> items)
	{
		return items.Select(e => new ResponseModel
		{
			Id = e.Id,
			TransporterId = e.TransporterId,
			OverallRating = e.OverallRating
		}).ToList();
	}
}
