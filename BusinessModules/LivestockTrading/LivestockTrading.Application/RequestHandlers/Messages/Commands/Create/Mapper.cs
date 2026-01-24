using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.Messages.Commands.Create;

public class Mapper
{
	public Message MapToEntity(RequestModel request)
	{
		return new Message
		{
			Id = Guid.NewGuid(),
			ConversationId = request.ConversationId,
			SenderUserId = request.SenderUserId,
			RecipientUserId = request.RecipientUserId,
			Content = request.Content,
			AttachmentUrls = request.AttachmentUrls,
			SentAt = DateTime.UtcNow,
			IsRead = false,
			CreatedAt = DateTime.UtcNow
		};
	}

	public ResponseModel MapToResponse(Message entity)
	{
		return new ResponseModel
		{
			Id = entity.Id,
			ConversationId = entity.ConversationId,
			SenderUserId = entity.SenderUserId,
			RecipientUserId = entity.RecipientUserId,
			Content = entity.Content,
			AttachmentUrls = entity.AttachmentUrls,
			SentAt = entity.SentAt,
			CreatedAt = entity.CreatedAt
		};
	}
}
