using LivestockTrading.Application.Extensions;
using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.Categories.Queries.All;

public class Mapper
{
	public List<ResponseModel> MapToResponse(List<Category> categories, Dictionary<Guid, int> productCounts, string languageCode = null)
	{
		// Build lookup for subcategory aggregation
		var categoryLookup = categories.ToDictionary(c => c.Id);

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
			ProductCount = GetTotalProductCount(c, productCounts, categoryLookup),
			CreatedAt = c.CreatedAt
		}).ToList();
	}

	/// <summary>
	/// Returns product count including all subcategories recursively
	/// </summary>
	private static int GetTotalProductCount(Category category, Dictionary<Guid, int> productCounts, Dictionary<Guid, Category> categoryLookup)
	{
		productCounts.TryGetValue(category.Id, out var directCount);

		var subCategoryCount = 0;
		if (category.SubCategories != null)
		{
			foreach (var sub in category.SubCategories.Where(sc => !sc.IsDeleted))
			{
				if (categoryLookup.TryGetValue(sub.Id, out var subCategory))
				{
					subCategoryCount += GetTotalProductCount(subCategory, productCounts, categoryLookup);
				}
				else if (productCounts.TryGetValue(sub.Id, out var subCount))
				{
					subCategoryCount += subCount;
				}
			}
		}

		return directCount + subCategoryCount;
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
