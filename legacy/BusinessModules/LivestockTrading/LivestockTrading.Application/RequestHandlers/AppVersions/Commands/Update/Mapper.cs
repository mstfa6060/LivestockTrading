using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.AppVersions.Commands.Update;

public class Mapper
{
	public void MapToEntity(RequestModel request, AppVersionConfig entity)
	{
		entity.Platform = request.Platform;
		entity.MinSupportedVersion = request.MinSupportedVersion;
		entity.LatestVersion = request.LatestVersion;
		entity.StoreUrl = request.StoreUrl;
		entity.UpdateMessage = request.UpdateMessage;
		entity.IsActive = request.IsActive;
		entity.UpdatedAt = DateTime.UtcNow;
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
			UpdatedAt = entity.UpdatedAt
		};
	}
}
