using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.FAQs.Queries.Pick;

public class Mapper
{
	public List<ResponseModel> MapToResponse(List<FAQ> items)
	{
		return items.Select(e => new ResponseModel
		{
			Id = e.Id,
			Question = e.Question,
			SortOrder = e.SortOrder
		}).ToList();
	}
}
