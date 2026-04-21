using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.Messages.Queries.Detail;

public class Mapper
{
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
			IsRead = entity.IsRead,
			ReadAt = entity.ReadAt,
			SentAt = entity.SentAt,
			CreatedAt = entity.CreatedAt
		};
	}
}
