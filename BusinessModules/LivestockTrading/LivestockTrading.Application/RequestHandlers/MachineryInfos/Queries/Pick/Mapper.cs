using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.MachineryInfos.Queries.Pick;

public class Mapper
{
	public List<ResponseModel> MapToResponse(List<MachineryInfo> items)
	{
		return items.Select(e => new ResponseModel
		{
			Id = e.Id,
			Model = e.Model,
			SerialNumber = e.SerialNumber
		}).ToList();
	}
}
