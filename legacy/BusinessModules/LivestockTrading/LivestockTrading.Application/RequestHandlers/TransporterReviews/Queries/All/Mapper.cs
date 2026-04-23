using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.TransporterReviews.Queries.All;

public class Mapper
{
	public List<ResponseModel> MapToResponse(List<TransporterReview> items)
	{
		return items.Select(e => new ResponseModel
		{
			Id = e.Id,
			TransporterId = e.TransporterId,
			UserId = e.UserId,
			OverallRating = e.OverallRating,
			TimelinessRating = e.TimelinessRating,
			CommunicationRating = e.CommunicationRating,
			CarefulHandlingRating = e.CarefulHandlingRating,
			ProfessionalismRating = e.ProfessionalismRating,
			IsApproved = e.IsApproved,
			CreatedAt = e.CreatedAt
		}).ToList();
	}
}
