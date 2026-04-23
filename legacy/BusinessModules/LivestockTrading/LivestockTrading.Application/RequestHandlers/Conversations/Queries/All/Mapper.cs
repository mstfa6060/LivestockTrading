using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.Conversations.Queries.All;

public class Mapper
{
	public List<ResponseModel> MapToResponse(List<Conversation> conversations)
	{
		return conversations.Select(c => new ResponseModel
		{
			Id = c.Id,
			ParticipantUserId1 = c.ParticipantUserId1,
			ParticipantUserId2 = c.ParticipantUserId2,
			ProductId = c.ProductId,
			Subject = c.Subject,
			Status = (int)c.Status,
			LastMessageAt = c.LastMessageAt,
			CreatedAt = c.CreatedAt
		}).ToList();
	}
}
