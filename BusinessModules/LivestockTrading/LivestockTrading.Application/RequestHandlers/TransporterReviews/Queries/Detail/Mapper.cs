using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.TransporterReviews.Queries.Detail;

public class Mapper
{
	public ResponseModel MapToResponse(TransporterReview entity)
	{
		return new ResponseModel
		{
			Id = entity.Id,
			TransporterId = entity.TransporterId,
			UserId = entity.UserId,
			TransportRequestId = entity.TransportRequestId,
			OverallRating = entity.OverallRating,
			TimelinessRating = entity.TimelinessRating,
			CommunicationRating = entity.CommunicationRating,
			CarefulHandlingRating = entity.CarefulHandlingRating,
			ProfessionalismRating = entity.ProfessionalismRating,
			Comment = entity.Comment,
			IsApproved = entity.IsApproved,
			CreatedAt = entity.CreatedAt,
			UpdatedAt = entity.UpdatedAt
		};
	}
}
