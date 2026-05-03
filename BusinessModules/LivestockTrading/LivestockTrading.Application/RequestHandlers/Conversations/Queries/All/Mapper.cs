using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.Conversations.Queries.All;

public class Mapper
{
	public List<ResponseModel> MapToResponse(List<Conversation> conversations, Dictionary<Guid, ConversationExtras> extras)
	{
		return conversations.Select(c =>
		{
			extras.TryGetValue(c.Id, out var ex);
			return new ResponseModel
			{
				Id = c.Id,
				ParticipantUserId1 = c.ParticipantUserId1,
				ParticipantUserId2 = c.ParticipantUserId2,
				ProductId = c.ProductId,
				Subject = c.Subject,
				Status = (int)c.Status,
				LastMessageAt = c.LastMessageAt,
				LastMessageContent = ex?.LastMessageContent,
				LastMessageSenderUserId = ex?.LastMessageSenderUserId,
				UnreadCount = ex?.UnreadCount ?? 0,
				CreatedAt = c.CreatedAt
			};
		}).ToList();
	}
}
