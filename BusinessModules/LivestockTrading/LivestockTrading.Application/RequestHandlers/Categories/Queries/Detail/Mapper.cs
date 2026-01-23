using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.Categories.Queries.Detail;

public class Mapper
{
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
			ParentCategoryName = entity.ParentCategory?.Name,
			NameTranslations = entity.NameTranslations,
			DescriptionTranslations = entity.DescriptionTranslations,
			AttributesTemplate = entity.AttributesTemplate,
			SubCategories = entity.SubCategories?.OrderBy(sc => sc.SortOrder).Select(sc => new ResponseModel.SubCategoryItem
			{
				Id = sc.Id,
				Name = sc.Name,
				Slug = sc.Slug,
				IconUrl = sc.IconUrl,
				SortOrder = sc.SortOrder,
				IsActive = sc.IsActive
			}).ToList(),
			CreatedAt = entity.CreatedAt,
			UpdatedAt = entity.UpdatedAt
		};
	}
}
