using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.Brands.Queries.Pick;

public class Mapper
{
	public List<ResponseModel> MapToResponse(List<Brand> brands)
	{
		return brands.Select(b => new ResponseModel
		{
			Id = b.Id,
			Name = b.Name,
			Slug = b.Slug
		}).ToList();
	}
}
