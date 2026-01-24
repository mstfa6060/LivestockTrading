using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.FAQs.Commands.Update;

public class Mapper
{
	public void MapToEntity(RequestModel request, FAQ entity)
	{
		entity.Question = request.Question;
		entity.Answer = request.Answer;
		entity.CategoryId = request.CategoryId;
		entity.ProductId = request.ProductId;
		entity.SortOrder = request.SortOrder;
		entity.IsActive = request.IsActive;
		entity.QuestionTranslations = request.QuestionTranslations;
		entity.AnswerTranslations = request.AnswerTranslations;
		entity.UpdatedAt = DateTime.UtcNow;
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
			UpdatedAt = entity.UpdatedAt
		};
	}
}
