using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.Products.Queries.Pick;

public class Mapper
{
	public List<ResponseModel> MapToResponse(List<Product> products)
	{
		return products.Select(p => new ResponseModel
		{
			Id = p.Id,
			Title = p.Title,
			Slug = p.Slug
		}).ToList();
	}
}
