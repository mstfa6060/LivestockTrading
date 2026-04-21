using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.Notifications.Commands.Create;

public class Mapper
{
	public Notification MapToEntity(RequestModel request)
	{
		return new Notification
		{
			Id = Guid.NewGuid(),
			UserId = request.UserId,
			Title = request.Title,
			Message = request.Message,
			Type = (NotificationType)request.Type,
			ActionUrl = request.ActionUrl,
			ActionData = request.ActionData,
			IsRead = false,
			SentAt = DateTime.UtcNow,
			CreatedAt = DateTime.UtcNow
		};
	}

	public ResponseModel MapToResponse(Notification entity)
	{
		return new ResponseModel
		{
			Id = entity.Id,
			UserId = entity.UserId,
			Title = entity.Title,
			Message = entity.Message,
			Type = (int)entity.Type,
			ActionUrl = entity.ActionUrl,
			ActionData = entity.ActionData,
			IsRead = entity.IsRead,
			SentAt = entity.SentAt,
			CreatedAt = entity.CreatedAt
		};
	}
}
