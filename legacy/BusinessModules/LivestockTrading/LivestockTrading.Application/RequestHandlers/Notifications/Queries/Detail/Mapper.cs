using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.Notifications.Queries.Detail;

public class Mapper
{
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
			SentAt = entity.SentAt,
			CreatedAt = entity.CreatedAt
		};
	}
}
