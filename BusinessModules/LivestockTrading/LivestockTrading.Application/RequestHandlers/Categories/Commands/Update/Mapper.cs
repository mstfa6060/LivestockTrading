using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.Categories.Commands.Update;

public class Mapper
{
	public void MapToEntity(RequestModel request, Category entity)
	{
		entity.Name = request.Name;
		entity.Slug = request.Slug;
		entity.Description = request.Description;
		entity.IconUrl = request.IconUrl;
		entity.SortOrder = request.SortOrder;
		entity.IsActive = request.IsActive;
		entity.ParentCategoryId = request.ParentCategoryId;
		entity.NameTranslations = request.NameTranslations;
		entity.DescriptionTranslations = request.DescriptionTranslations;
		entity.AttributesTemplate = request.AttributesTemplate;
		entity.UpdatedAt = DateTime.UtcNow;
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
			UpdatedAt = entity.UpdatedAt
		};
	}
}
