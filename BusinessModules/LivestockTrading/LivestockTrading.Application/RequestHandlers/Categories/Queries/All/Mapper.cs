using LivestockTrading.Application.Extensions;
using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.Categories.Queries.All;

public class Mapper
{
	public List<ResponseModel> MapToResponse(List<Category> categories, string languageCode = null)
	{
		return categories.Select(c => new ResponseModel
		{
			Id = c.Id,
			Name = GetTranslatedName(c, languageCode),
			Slug = c.Slug,
			Description = GetTranslatedDescription(c, languageCode),
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
}
