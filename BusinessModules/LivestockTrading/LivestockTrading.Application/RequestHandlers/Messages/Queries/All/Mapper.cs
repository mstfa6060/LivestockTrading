using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.Messages.Queries.All;

public class Mapper
{
	public List<ResponseModel> MapToResponse(List<Message> messages)
	{
		return messages.Select(m => new ResponseModel
		{
			Id = m.Id,
			ConversationId = m.ConversationId,
			SenderUserId = m.SenderUserId,
			RecipientUserId = m.RecipientUserId,
			Content = m.Content,
			IsRead = m.IsRead,
			SentAt = m.SentAt,
			CreatedAt = m.CreatedAt
		}).ToList();
	}
}
