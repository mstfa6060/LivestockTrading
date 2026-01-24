using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.Notifications.Queries.Pick;

public class Mapper
{
	public List<ResponseModel> MapToResponse(List<Notification> items)
	{
		return items.Select(entity => new ResponseModel
		{
			Id = entity.Id,
			Title = entity.Title,
			Type = (int)entity.Type
		}).ToList();
	}
}
