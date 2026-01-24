using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.Conversations.Commands.Create;

public class Mapper
{
	public Conversation MapToEntity(RequestModel request)
	{
		return new Conversation
		{
			Id = Guid.NewGuid(),
			ParticipantUserId1 = request.ParticipantUserId1,
			ParticipantUserId2 = request.ParticipantUserId2,
			ProductId = request.ProductId,
			OrderId = request.OrderId,
			Subject = request.Subject,
			Status = request.Status,
			LastMessageAt = DateTime.UtcNow,
			CreatedAt = DateTime.UtcNow
		};
	}

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
			Status = entity.Status,
			CreatedAt = entity.CreatedAt
		};
	}
}
