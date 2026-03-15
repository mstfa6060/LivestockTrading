using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.ProductBoosts.Queries.All;

public class Mapper
{
	public List<ResponseModel> MapToResponse(List<ProductBoost> boosts)
	{
		return boosts.Select(b => new ResponseModel
		{
			Id = b.Id,
			ProductId = b.ProductId,
			ProductTitle = b.Product?.Title,
			BoostPackageId = b.BoostPackageId,
			BoostPackageName = b.BoostPackage?.Name,
			BoostType = b.BoostPackage != null ? (int)b.BoostPackage.BoostType : 0,
			StartedAt = b.StartedAt,
			ExpiresAt = b.ExpiresAt,
			BoostScore = b.BoostScore,
			IsActive = b.IsActive && b.ExpiresAt > DateTime.UtcNow,
			IsExpired = b.ExpiresAt <= DateTime.UtcNow,
			CreatedAt = b.CreatedAt
		}).ToList();
	}
}
