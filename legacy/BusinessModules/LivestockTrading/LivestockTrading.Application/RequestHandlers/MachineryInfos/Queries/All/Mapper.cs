using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.MachineryInfos.Queries.All;

public class Mapper
{
	public List<ResponseModel> MapToResponse(List<MachineryInfo> items)
	{
		return items.Select(e => new ResponseModel
		{
			Id = e.Id,
			ProductId = e.ProductId,
			Type = (int)e.Type,
			Model = e.Model,
			YearOfManufacture = e.YearOfManufacture,
			PowerHp = e.PowerHp,
			HoursUsed = e.HoursUsed,
			HasWarranty = e.HasWarranty,
			CreatedAt = e.CreatedAt
		}).ToList();
	}
}
