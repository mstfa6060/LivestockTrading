using LivestockTrading.Application.Extensions;
using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.Categories.Queries.Pick;

public class Mapper
{
	public List<ResponseModel> MapToResponse(List<Category> categories, string languageCode = null)
	{
		return categories.Select(c => new ResponseModel
		{
			Id = c.Id,
			Name = GetTranslatedName(c, languageCode),
			Slug = c.Slug
		}).ToList();
	}

	private static string GetTranslatedName(Category c, string languageCode)
	{
		if (string.IsNullOrWhiteSpace(languageCode))
			return c.Name;

		return TranslationHelper.GetTranslation(c.NameTranslations, languageCode, c.Name);
	}
}
