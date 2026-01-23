using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.Categories.Queries.All;

public class Mapper
{
	public List<ResponseModel> MapToResponse(List<Category> categories)
	{
		return categories.Select(c => new ResponseModel
		{
			Id = c.Id,
			Name = c.Name,
			Slug = c.Slug,
			Description = c.Description,
			IconUrl = c.IconUrl,
			SortOrder = c.SortOrder,
			IsActive = c.IsActive,
			ParentCategoryId = c.ParentCategoryId,
			NameTranslations = c.NameTranslations,
			DescriptionTranslations = c.DescriptionTranslations,
			AttributesTemplate = c.AttributesTemplate,
			SubCategoryCount = c.SubCategories?.Count(sc => !sc.IsDeleted) ?? 0,
			CreatedAt = c.CreatedAt
		}).ToList();
	}
}
