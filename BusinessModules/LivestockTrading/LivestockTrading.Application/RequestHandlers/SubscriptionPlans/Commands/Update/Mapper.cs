using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.SubscriptionPlans.Commands.Update;

public class Mapper
{
	public void MapToEntity(RequestModel request, SubscriptionPlan entity)
	{
		entity.Name = request.Name;
		entity.Description = request.Description;
		entity.NameTranslations = request.NameTranslations;
		entity.DescriptionTranslations = request.DescriptionTranslations;
		entity.PriceMonthly = request.PriceMonthly;
		entity.PriceYearly = request.PriceYearly;
		entity.Currency = request.Currency;
		entity.MaxActiveListings = request.MaxActiveListings;
		entity.MaxPhotosPerListing = request.MaxPhotosPerListing;
		entity.MonthlyBoostCredits = request.MonthlyBoostCredits;
		entity.HasDetailedAnalytics = request.HasDetailedAnalytics;
		entity.HasPrioritySupport = request.HasPrioritySupport;
		entity.HasFeaturedBadge = request.HasFeaturedBadge;
		entity.IsActive = request.IsActive;
		entity.AppleProductIdMonthly = request.AppleProductIdMonthly;
		entity.AppleProductIdYearly = request.AppleProductIdYearly;
		entity.GoogleProductIdMonthly = request.GoogleProductIdMonthly;
		entity.GoogleProductIdYearly = request.GoogleProductIdYearly;
		entity.SortOrder = request.SortOrder;
		entity.UpdatedAt = DateTime.UtcNow;
	}

	public ResponseModel MapToResponse(SubscriptionPlan entity)
	{
		return new ResponseModel
		{
			Id = entity.Id,
			Name = entity.Name,
			Description = entity.Description,
			NameTranslations = entity.NameTranslations,
			DescriptionTranslations = entity.DescriptionTranslations,
			TargetType = (int)entity.TargetType,
			Tier = (int)entity.Tier,
			PriceMonthly = entity.PriceMonthly,
			PriceYearly = entity.PriceYearly,
			Currency = entity.Currency,
			AppleProductIdMonthly = entity.AppleProductIdMonthly,
			AppleProductIdYearly = entity.AppleProductIdYearly,
			GoogleProductIdMonthly = entity.GoogleProductIdMonthly,
			GoogleProductIdYearly = entity.GoogleProductIdYearly,
			MaxActiveListings = entity.MaxActiveListings,
			MaxPhotosPerListing = entity.MaxPhotosPerListing,
			MonthlyBoostCredits = entity.MonthlyBoostCredits,
			HasDetailedAnalytics = entity.HasDetailedAnalytics,
			HasPrioritySupport = entity.HasPrioritySupport,
			HasFeaturedBadge = entity.HasFeaturedBadge,
			SortOrder = entity.SortOrder,
			IsActive = entity.IsActive,
			CreatedAt = entity.CreatedAt,
			UpdatedAt = entity.UpdatedAt
		};
	}
}
