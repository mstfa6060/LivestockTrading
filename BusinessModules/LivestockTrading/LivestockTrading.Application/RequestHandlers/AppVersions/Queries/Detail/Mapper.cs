using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.AppVersions.Queries.Detail;

public class Mapper
{
	public ResponseModel MapToResponse(AppVersionConfig entity)
	{
		return new ResponseModel
		{
			Id = entity.Id,
			Platform = entity.Platform,
			MinSupportedVersion = entity.MinSupportedVersion,
			LatestVersion = entity.LatestVersion,
			StoreUrl = entity.StoreUrl,
			UpdateMessage = entity.UpdateMessage,
			IsActive = entity.IsActive,
			CreatedAt = entity.CreatedAt,
			UpdatedAt = entity.UpdatedAt
		};
	}
}
