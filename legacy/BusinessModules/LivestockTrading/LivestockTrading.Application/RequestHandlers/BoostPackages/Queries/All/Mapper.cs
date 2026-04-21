using LivestockTrading.Domain.Entities;
using LivestockTrading.Application.Extensions;

namespace LivestockTrading.Application.RequestHandlers.BoostPackages.Queries.All;

public class Mapper
{
	public List<ResponseModel> MapToResponse(List<BoostPackage> packages, string languageCode = null)
	{
		return packages.Select(p => new ResponseModel
		{
			Id = p.Id,
			Name = !string.IsNullOrWhiteSpace(languageCode)
				? TranslationHelper.GetTranslation(p.NameTranslations, languageCode, p.Name)
				: p.Name,
			Description = !string.IsNullOrWhiteSpace(languageCode)
				? TranslationHelper.GetTranslation(p.DescriptionTranslations, languageCode, p.Description)
				: p.Description,
			DurationHours = p.DurationHours,
			Price = p.Price,
			Currency = p.Currency,
			AppleProductId = p.AppleProductId,
			GoogleProductId = p.GoogleProductId,
			BoostType = (int)p.BoostType,
			BoostScore = p.BoostScore,
			SortOrder = p.SortOrder,
			IsActive = p.IsActive,
			CreatedAt = p.CreatedAt
		}).ToList();
	}
}
