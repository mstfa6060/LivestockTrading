using LivestockTrading.Domain.Entities;
using LivestockTrading.Application.Extensions;

namespace LivestockTrading.Application.RequestHandlers.SubscriptionPlans.Queries.Detail;

public class Mapper
{
	public ResponseModel MapToResponse(SubscriptionPlan p, string languageCode = null)
	{
		return new ResponseModel
		{
			Id = p.Id,
			Name = !string.IsNullOrWhiteSpace(languageCode)
				? TranslationHelper.GetTranslation(p.NameTranslations, languageCode, p.Name)
				: p.Name,
			Description = !string.IsNullOrWhiteSpace(languageCode)
				? TranslationHelper.GetTranslation(p.DescriptionTranslations, languageCode, p.Description)
				: p.Description,
			TargetType = (int)p.TargetType,
			Tier = (int)p.Tier,
			PriceMonthly = p.PriceMonthly,
			PriceYearly = p.PriceYearly,
			Currency = p.Currency,
			AppleProductIdMonthly = p.AppleProductIdMonthly,
			AppleProductIdYearly = p.AppleProductIdYearly,
			GoogleProductIdMonthly = p.GoogleProductIdMonthly,
			GoogleProductIdYearly = p.GoogleProductIdYearly,
			MaxActiveListings = p.MaxActiveListings,
			MaxPhotosPerListing = p.MaxPhotosPerListing,
			MonthlyBoostCredits = p.MonthlyBoostCredits,
			HasDetailedAnalytics = p.HasDetailedAnalytics,
			HasPrioritySupport = p.HasPrioritySupport,
			HasFeaturedBadge = p.HasFeaturedBadge,
			SortOrder = p.SortOrder,
			IsActive = p.IsActive,
			CreatedAt = p.CreatedAt,
			UpdatedAt = p.UpdatedAt
		};
	}
}
