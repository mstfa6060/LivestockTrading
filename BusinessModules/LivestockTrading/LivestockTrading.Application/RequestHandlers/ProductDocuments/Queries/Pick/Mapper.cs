using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.ProductDocuments.Queries.Pick;

public class Mapper
{
	public List<ResponseModel> MapToResponse(List<ProductDocument> items)
	{
		return items.Select(e => new ResponseModel
		{
			Id = e.Id,
			Title = e.Title,
			FileName = e.FileName
		}).ToList();
	}
}
