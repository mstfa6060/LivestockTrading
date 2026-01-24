using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.ProductVideos.Queries.Pick;

public class Mapper
{
	public List<ResponseModel> MapToResponse(List<ProductVideo> items)
	{
		return items.Select(e => new ResponseModel
		{
			Id = e.Id,
			Title = e.Title,
			VideoUrl = e.VideoUrl
		}).ToList();
	}
}
