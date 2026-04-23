using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.TransporterReviews.Commands.Create;

public class Mapper
{
	public TransporterReview MapToEntity(RequestModel request)
	{
		return new TransporterReview
		{
			Id = Guid.NewGuid(),
			TransporterId = request.TransporterId,
			UserId = request.UserId,
			TransportRequestId = request.TransportRequestId,
			OverallRating = request.OverallRating,
			TimelinessRating = request.TimelinessRating,
			CommunicationRating = request.CommunicationRating,
			CarefulHandlingRating = request.CarefulHandlingRating,
			ProfessionalismRating = request.ProfessionalismRating,
			Comment = request.Comment,
			CreatedAt = DateTime.UtcNow
		};
	}

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
			CreatedAt = entity.CreatedAt
		};
	}
}
