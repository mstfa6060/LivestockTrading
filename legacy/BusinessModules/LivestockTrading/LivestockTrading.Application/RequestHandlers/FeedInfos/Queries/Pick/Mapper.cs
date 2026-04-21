using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.FeedInfos.Queries.Pick;

public class Mapper
{
	public List<ResponseModel> MapToResponse(List<FeedInfo> items)
	{
		return items.Select(e => new ResponseModel
		{
			Id = e.Id,
			TargetAnimal = e.TargetAnimal,
			Type = (int)e.Type
		}).ToList();
	}
}
