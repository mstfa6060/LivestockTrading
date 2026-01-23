using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.Categories.Queries.Pick;

public class Mapper
{
	public List<ResponseModel> MapToResponse(List<Category> categories)
	{
		return categories.Select(c => new ResponseModel
		{
			Id = c.Id,
			Name = c.Name,
			Slug = c.Slug
		}).ToList();
	}
}
