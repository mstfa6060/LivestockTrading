using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.FAQs.Queries.All;

public class Mapper
{
	public List<ResponseModel> MapToResponse(List<FAQ> items)
	{
		return items.Select(e => new ResponseModel
		{
			Id = e.Id,
			Question = e.Question,
			Answer = e.Answer,
			CategoryId = e.CategoryId,
			ProductId = e.ProductId,
			SortOrder = e.SortOrder,
			IsActive = e.IsActive,
			QuestionTranslations = e.QuestionTranslations,
			AnswerTranslations = e.AnswerTranslations,
			CreatedAt = e.CreatedAt
		}).ToList();
	}
}
