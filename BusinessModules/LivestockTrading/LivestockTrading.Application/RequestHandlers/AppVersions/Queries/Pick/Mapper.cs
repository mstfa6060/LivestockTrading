using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.AppVersions.Queries.Pick;

public class Mapper
{
	public List<ResponseModel> MapToResponse(List<AppVersionConfig> items)
	{
		return items.Select(e => new ResponseModel
		{
			Id = e.Id,
			Platform = e.Platform,
			LatestVersion = e.LatestVersion,
			UpdateMessage = e.UpdateMessage
		}).ToList();
	}
}
