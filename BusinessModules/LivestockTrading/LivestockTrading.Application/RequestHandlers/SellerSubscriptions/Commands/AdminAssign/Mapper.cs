using Common.Services.Auth.CurrentUser;
using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.SellerSubscriptions.Commands.AdminAssign;

public class Mapper
{
	public SellerSubscription MapToEntity(RequestModel request, Guid actorUserId)
	{
		var period = (SubscriptionPeriod)request.Period;
		var now = DateTime.UtcNow;

		// OriginalTransactionId'yi audit izi olarak kullanıyoruz. Bu kayıtlar gerçek IAP/web
		// satışı değil — admin tarafından atandığı belli olsun ki muhasebe/raporlama tarafı
		// karistirmasin.
		var transactionId = $"admin-assign:{actorUserId}:{now:O}";
		if (!string.IsNullOrWhiteSpace(request.Note))
			transactionId += $":{request.Note}";

		return new SellerSubscription
		{
			Id = Guid.NewGuid(),
			SellerId = request.SellerId,
			SubscriptionPlanId = request.SubscriptionPlanId,
			Status = SubscriptionStatus.Active,
			Period = period,
			Platform = SubscriptionPlatform.Web,
			OriginalTransactionId = transactionId,
			StartedAt = now,
			ExpiresAt = period == SubscriptionPeriod.Monthly
				? now.AddMonths(1)
				: now.AddYears(1),
			AutoRenew = false,
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
