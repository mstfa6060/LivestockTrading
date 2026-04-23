using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.Notifications.Commands.Update;

public class Mapper
{
	public void MapToEntity(RequestModel request, Notification entity)
	{
		entity.Title = request.Title;
		entity.Message = request.Message;
		entity.Type = (NotificationType)request.Type;
		entity.ActionUrl = request.ActionUrl;
		entity.ActionData = request.ActionData;
		entity.IsRead = request.IsRead;
		entity.ReadAt = request.IsRead ? DateTime.UtcNow : null;
		entity.UpdatedAt = DateTime.UtcNow;
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
			ReadAt = entity.ReadAt,
			UpdatedAt = entity.UpdatedAt
		};
	}
}
