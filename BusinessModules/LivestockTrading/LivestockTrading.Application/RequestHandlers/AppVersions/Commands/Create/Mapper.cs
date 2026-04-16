using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.AppVersions.Commands.Create;

public class Mapper
{
	public AppVersionConfig MapToEntity(RequestModel request)
	{
		return new AppVersionConfig
		{
			Id = Guid.NewGuid(),
			Platform = request.Platform,
			MinSupportedVersion = request.MinSupportedVersion,
			LatestVersion = request.LatestVersion,
			StoreUrl = request.StoreUrl,
			UpdateMessage = request.UpdateMessage,
			IsActive = request.IsActive,
			CreatedAt = DateTime.UtcNow
		};
	}

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
			CreatedAt = entity.CreatedAt
		};
	}
}
