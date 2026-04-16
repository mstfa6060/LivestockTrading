using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.AppVersions.Queries.All;

public class Mapper
{
	public List<ResponseModel> MapToResponse(List<AppVersionConfig> items)
	{
		return items.Select(e => new ResponseModel
		{
			Id = e.Id,
			Platform = e.Platform,
			MinSupportedVersion = e.MinSupportedVersion,
			LatestVersion = e.LatestVersion,
			StoreUrl = e.StoreUrl,
			UpdateMessage = e.UpdateMessage,
			IsActive = e.IsActive,
			CreatedAt = e.CreatedAt,
			UpdatedAt = e.UpdatedAt
		}).ToList();
	}
}
