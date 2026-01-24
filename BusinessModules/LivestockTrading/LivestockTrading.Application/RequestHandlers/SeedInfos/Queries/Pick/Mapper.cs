using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.SeedInfos.Queries.Pick;

public class Mapper
{
	public List<ResponseModel> MapToResponse(List<SeedInfo> items)
	{
		return items.Select(e => new ResponseModel
		{
			Id = e.Id,
			Variety = e.Variety,
			Type = (int)e.Type
		}).ToList();
	}
}
