using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.Categories.Commands.Create;

public class Mapper
{
	public Category MapToEntity(RequestModel request)
	{
		return new Category
		{
			Id = Guid.NewGuid(),
			Name = request.Name,
			Slug = request.Slug,
			Description = request.Description,
			IconUrl = request.IconUrl,
			SortOrder = request.SortOrder,
			IsActive = request.IsActive,
			ParentCategoryId = request.ParentCategoryId,
			NameTranslations = request.NameTranslations,
			DescriptionTranslations = request.DescriptionTranslations,
			AttributesTemplate = request.AttributesTemplate,
			CreatedAt = DateTime.UtcNow
		};
	}

	public ResponseModel MapToResponse(Category entity)
	{
		return new ResponseModel
		{
			Id = entity.Id,
			Name = entity.Name,
			Slug = entity.Slug,
			Description = entity.Description,
			IconUrl = entity.IconUrl,
			SortOrder = entity.SortOrder,
			IsActive = entity.IsActive,
			ParentCategoryId = entity.ParentCategoryId,
			NameTranslations = entity.NameTranslations,
			DescriptionTranslations = entity.DescriptionTranslations,
			AttributesTemplate = entity.AttributesTemplate,
			CreatedAt = entity.CreatedAt
		};
	}
}
