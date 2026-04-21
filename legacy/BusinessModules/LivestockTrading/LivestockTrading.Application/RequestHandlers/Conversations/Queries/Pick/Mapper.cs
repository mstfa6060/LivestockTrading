using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.Conversations.Queries.Pick;

public class Mapper
{
	public List<ResponseModel> MapToResponse(List<Conversation> conversations)
	{
		return conversations.Select(c => new ResponseModel
		{
			Id = c.Id,
			Subject = c.Subject,
			LastMessageAt = c.LastMessageAt
		}).ToList();
	}
}
