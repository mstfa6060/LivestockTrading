using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.ProductBoosts.Commands.Create;

public class Mapper
{
	public ProductBoost MapToEntity(RequestModel request, BoostPackage package)
	{
		var now = DateTime.UtcNow;
		return new ProductBoost
		{
			Id = Guid.NewGuid(),
			ProductId = request.ProductId,
			SellerId = request.SellerId,
			BoostPackageId = request.BoostPackageId,
			StartedAt = now,
			ExpiresAt = now.AddHours(package.DurationHours),
			IsActive = true,
			BoostScore = package.BoostScore,
			CreatedAt = now
		};
	}

	public ResponseModel MapToResponse(ProductBoost entity)
	{
		return new ResponseModel
		{
			Id = entity.Id,
			ProductId = entity.ProductId,
			SellerId = entity.SellerId,
			BoostPackageId = entity.BoostPackageId,
			StartedAt = entity.StartedAt,
			ExpiresAt = entity.ExpiresAt,
			BoostScore = entity.BoostScore,
			IsActive = entity.IsActive,
			CreatedAt = entity.CreatedAt
		};
	}
}
