using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.Notifications.Queries.All;

public class Mapper
{
	public List<ResponseModel> MapToResponse(List<Notification> items)
	{
		return items.Select(entity => new ResponseModel
		{
			Id = entity.Id,
			UserId = entity.UserId,
			Title = entity.Title,
			Message = entity.Message,
			Type = (int)entity.Type,
			IsRead = entity.IsRead,
			SentAt = entity.SentAt,
			CreatedAt = entity.CreatedAt
		}).ToList();
	}
}
