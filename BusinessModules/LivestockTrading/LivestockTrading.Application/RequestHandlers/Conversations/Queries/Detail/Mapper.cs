using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.Conversations.Queries.Detail;

public class Mapper
{
	public ResponseModel MapToResponse(Conversation entity)
	{
		return new ResponseModel
		{
			Id = entity.Id,
			ParticipantUserId1 = entity.ParticipantUserId1,
			ParticipantUserId2 = entity.ParticipantUserId2,
			ProductId = entity.ProductId,
			OrderId = entity.OrderId,
			Subject = entity.Subject,
			Status = (int)entity.Status,
			LastMessageAt = entity.LastMessageAt,
			CreatedAt = entity.CreatedAt,
			UpdatedAt = entity.UpdatedAt
		};
	}
}
