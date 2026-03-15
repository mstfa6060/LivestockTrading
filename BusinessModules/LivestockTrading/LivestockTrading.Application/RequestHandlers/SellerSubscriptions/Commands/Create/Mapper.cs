using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.SellerSubscriptions.Commands.Create;

public class Mapper
{
	public SellerSubscription MapToEntity(RequestModel request)
	{
		var period = (SubscriptionPeriod)request.Period;
		var now = DateTime.UtcNow;

		return new SellerSubscription
		{
			Id = Guid.NewGuid(),
			SellerId = request.SellerId,
			SubscriptionPlanId = request.SubscriptionPlanId,
			Status = SubscriptionStatus.Active,
			Period = period,
			Platform = (SubscriptionPlatform)request.Platform,
			OriginalTransactionId = request.StoreTransactionId,
			StartedAt = now,
			ExpiresAt = period == SubscriptionPeriod.Monthly
				? now.AddMonths(1)
				: now.AddYears(1),
			AutoRenew = true,
			CreatedAt = now
		};
	}

	public ResponseModel MapToResponse(SellerSubscription entity)
	{
		return new ResponseModel
		{
			Id = entity.Id,
			SellerId = entity.SellerId,
			SubscriptionPlanId = entity.SubscriptionPlanId,
			Status = (int)entity.Status,
			Period = (int)entity.Period,
			Platform = (int)entity.Platform,
			StartedAt = entity.StartedAt,
			ExpiresAt = entity.ExpiresAt,
			AutoRenew = entity.AutoRenew,
			CreatedAt = entity.CreatedAt
		};
	}
}
