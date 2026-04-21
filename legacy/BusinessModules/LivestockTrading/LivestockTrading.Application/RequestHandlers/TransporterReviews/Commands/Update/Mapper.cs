using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.TransporterReviews.Commands.Update;

public class Mapper
{
	public void MapToEntity(RequestModel request, TransporterReview entity)
	{
		entity.TransporterId = request.TransporterId;
		entity.UserId = request.UserId;
		entity.TransportRequestId = request.TransportRequestId;
		entity.OverallRating = request.OverallRating;
		entity.TimelinessRating = request.TimelinessRating;
		entity.CommunicationRating = request.CommunicationRating;
		entity.CarefulHandlingRating = request.CarefulHandlingRating;
		entity.ProfessionalismRating = request.ProfessionalismRating;
		entity.Comment = request.Comment;
		entity.UpdatedAt = DateTime.UtcNow;
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
			UpdatedAt = entity.UpdatedAt
		};
	}
}
