using LivestockTrading.Application.Extensions;
using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.Categories.Queries.Detail;

public class Mapper
{
	public ResponseModel MapToResponse(Category entity, string languageCode = null)
	{
		return new ResponseModel
		{
			Id = entity.Id,
			Name = GetTranslatedName(entity, languageCode),
			Slug = entity.Slug,
			Description = GetTranslatedDescription(entity, languageCode),
			IconUrl = entity.IconUrl,
			SortOrder = entity.SortOrder,
			IsActive = entity.IsActive,
			ParentCategoryId = entity.ParentCategoryId,
			ParentCategoryName = GetTranslatedParentName(entity.ParentCategory, languageCode),
			NameTranslations = entity.NameTranslations,
			DescriptionTranslations = entity.DescriptionTranslations,
			AttributesTemplate = entity.AttributesTemplate,
			SubCategories = entity.SubCategories?.OrderBy(sc => sc.SortOrder).Select(sc => new ResponseModel.SubCategoryItem
			{
				Id = sc.Id,
				Name = GetTranslatedName(sc, languageCode),
				Slug = sc.Slug,
				IconUrl = sc.IconUrl,
				SortOrder = sc.SortOrder,
				IsActive = sc.IsActive
			}).ToList(),
			CreatedAt = entity.CreatedAt,
			UpdatedAt = entity.UpdatedAt
		};
	}

	private static string GetTranslatedName(Category c, string languageCode)
	{
		if (string.IsNullOrWhiteSpace(languageCode))
			return c.Name;

		return TranslationHelper.GetTranslation(c.NameTranslations, languageCode, c.Name);
	}

	private static string GetTranslatedDescription(Category c, string languageCode)
	{
		if (string.IsNullOrWhiteSpace(languageCode))
			return c.Description;

		return TranslationHelper.GetTranslation(c.DescriptionTranslations, languageCode, c.Description);
	}

	private static string GetTranslatedParentName(Category parent, string languageCode)
	{
		if (parent == null)
			return null;

		return GetTranslatedName(parent, languageCode);
	}
}
