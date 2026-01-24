using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.Messages.Queries.Pick;

public class Mapper
{
	public List<ResponseModel> MapToResponse(List<Message> messages)
	{
		return messages.Select(m => new ResponseModel
		{
			Id = m.Id,
			Content = m.Content,
			SentAt = m.SentAt
		}).ToList();
	}
}
