using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.ProductDocuments.Queries.All;

public class Mapper
{
	public List<ResponseModel> MapToResponse(List<ProductDocument> items)
	{
		return items.Select(e => new ResponseModel
		{
			Id = e.Id,
			ProductId = e.ProductId,
			DocumentUrl = e.DocumentUrl,
			FileName = e.FileName,
			Title = e.Title,
			Type = (int)e.Type,
			FileSizeBytes = e.FileSizeBytes,
			MimeType = e.MimeType,
			SortOrder = e.SortOrder,
			CreatedAt = e.CreatedAt
		}).ToList();
	}
}
