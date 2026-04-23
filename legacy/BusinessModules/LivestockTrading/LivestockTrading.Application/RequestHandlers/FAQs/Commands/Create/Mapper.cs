using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.FAQs.Commands.Create;

public class Mapper
{
	public FAQ MapToEntity(RequestModel request)
	{
		return new FAQ
		{
			Id = Guid.NewGuid(),
			Question = request.Question,
			Answer = request.Answer,
			CategoryId = request.CategoryId,
			ProductId = request.ProductId,
			SortOrder = request.SortOrder,
			IsActive = request.IsActive,
			QuestionTranslations = request.QuestionTranslations,
			AnswerTranslations = request.AnswerTranslations,
			CreatedAt = DateTime.UtcNow
		};
	}

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
			CreatedAt = entity.CreatedAt
		};
	}
}
