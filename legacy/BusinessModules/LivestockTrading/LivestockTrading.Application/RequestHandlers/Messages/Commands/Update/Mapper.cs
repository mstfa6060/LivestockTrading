using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.Messages.Commands.Update;

public class Mapper
{
	public void MapToEntity(RequestModel request, Message entity)
	{
		entity.Content = request.Content;
		entity.AttachmentUrls = request.AttachmentUrls;
		entity.IsRead = request.IsRead;
		entity.ReadAt = request.ReadAt;
		entity.UpdatedAt = DateTime.UtcNow;
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
			IsRead = entity.IsRead,
			ReadAt = entity.ReadAt,
			UpdatedAt = entity.UpdatedAt
		};
	}
}
