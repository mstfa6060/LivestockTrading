using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.Conversations.Commands.Update;

public class Mapper
{
	public void MapToEntity(RequestModel request, Conversation entity)
	{
		entity.ParticipantUserId1 = request.ParticipantUserId1;
		entity.ParticipantUserId2 = request.ParticipantUserId2;
		entity.ProductId = request.ProductId;
		entity.OrderId = request.OrderId;
		entity.Subject = request.Subject;
		entity.Status = request.Status;
		entity.UpdatedAt = DateTime.UtcNow;
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
			UpdatedAt = entity.UpdatedAt
		};
	}
}
