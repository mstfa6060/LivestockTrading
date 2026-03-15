using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.SellerSubscriptions.Queries.Detail;

public class Mapper
{
	public ResponseModel MapToResponse(SellerSubscription entity, int activeListings = 0)
	{
		var plan = entity.SubscriptionPlan;
		var maxListings = plan?.MaxActiveListings ?? 3;

		return new ResponseModel
		{
			Id = entity.Id,
			SellerId = entity.SellerId,
			SubscriptionPlanId = entity.SubscriptionPlanId,
			PlanName = plan?.Name,
			PlanTier = plan != null ? (int)plan.Tier : 0,
			Status = (int)entity.Status,
			Period = (int)entity.Period,
			Platform = (int)entity.Platform,
			StartedAt = entity.StartedAt,
			ExpiresAt = entity.ExpiresAt,
			AutoRenew = entity.AutoRenew,
			MaxActiveListings = maxListings,
			MaxPhotosPerListing = plan?.MaxPhotosPerListing ?? 3,
			MonthlyBoostCredits = plan?.MonthlyBoostCredits ?? 0,
			HasDetailedAnalytics = plan?.HasDetailedAnalytics ?? false,
			CurrentActiveListings = activeListings,
			RemainingListings = maxListings == 0 ? -1 : Math.Max(0, maxListings - activeListings),
			CreatedAt = entity.CreatedAt
		};
	}
}
