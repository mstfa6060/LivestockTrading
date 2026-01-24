using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.ProductVariants.Queries.Pick;

public class Mapper
{
	public List<ResponseModel> MapToResponse(List<ProductVariant> items)
	{
		return items.Select(e => new ResponseModel
		{
			Id = e.Id,
			Name = e.Name,
			SKU = e.SKU
		}).ToList();
	}
}
