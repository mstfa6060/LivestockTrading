using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.FAQs.Queries.Detail;

public class Mapper
{
	public ResponseModel MapToResponse(FAQ entity)
	{
		return new ResponseModel
		{
			Id = entity.Id,
			Question = entity.Question,
			Answer = entity.Answer,
			CategoryId = entity.CategoryId,
			ProductId = entity.ProductId,
			SortOrder = entity.SortOrder,
			IsActive = entity.IsActive,
			QuestionTranslations = entity.QuestionTranslations,
			AnswerTranslations = entity.AnswerTranslations,
			CreatedAt = entity.CreatedAt,
			UpdatedAt = entity.UpdatedAt
		};
	}
}
