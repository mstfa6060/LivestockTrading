using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.ProductImages.Queries.Pick;

public class Mapper
{
	public List<ResponseModel> MapToResponse(List<ProductImage> items)
	{
		return items.Select(e => new ResponseModel
		{
			Id = e.Id,
			ImageUrl = e.ImageUrl,
			AltText = e.AltText
		}).ToList();
	}
}
