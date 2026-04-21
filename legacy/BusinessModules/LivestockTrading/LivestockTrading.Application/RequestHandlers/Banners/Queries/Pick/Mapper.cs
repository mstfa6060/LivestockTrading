using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.Banners.Queries.Pick;

public class Mapper
{
	public List<ResponseModel> MapToResponse(List<Banner> items)
	{
		return items.Select(e => new ResponseModel
		{
			Id = e.Id,
			Title = e.Title,
			Position = (int)e.Position
		}).ToList();
	}
}
