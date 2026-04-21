using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.SellerSubscriptions.Commands.Update;

public class Mapper
{
	public void MapToEntity(RequestModel request, SellerSubscription entity)
	{
		if (request.Status.HasValue)
		{
			entity.Status = (SubscriptionStatus)request.Status.Value;
			if (entity.Status == SubscriptionStatus.Cancelled)
				entity.CancelledAt = DateTime.UtcNow;
		}

		if (request.AutoRenew.HasValue)
			entity.AutoRenew = request.AutoRenew.Value;

		entity.UpdatedAt = DateTime.UtcNow;
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
			CancelledAt = entity.CancelledAt,
			CreatedAt = entity.CreatedAt,
			UpdatedAt = entity.UpdatedAt
		};
	}
}
