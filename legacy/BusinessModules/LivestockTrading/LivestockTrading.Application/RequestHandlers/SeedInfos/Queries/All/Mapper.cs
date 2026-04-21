using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.SeedInfos.Queries.All;

public class Mapper
{
	public List<ResponseModel> MapToResponse(List<SeedInfo> items)
	{
		return items.Select(e => new ResponseModel
		{
			Id = e.Id,
			ProductId = e.ProductId,
			Type = (int)e.Type,
			Variety = e.Variety,
			ScientificName = e.ScientificName,
			GerminationRate = e.GerminationRate,
			DaysToMaturity = e.DaysToMaturity,
			IsOrganic = e.IsOrganic,
			IsHybrid = e.IsHybrid,
			CreatedAt = e.CreatedAt
		}).ToList();
	}
}
