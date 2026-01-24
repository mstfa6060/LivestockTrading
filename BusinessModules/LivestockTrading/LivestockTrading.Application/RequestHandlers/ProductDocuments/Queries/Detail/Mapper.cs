using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.ProductDocuments.Queries.Detail;

public class Mapper
{
	public ResponseModel MapToResponse(ProductDocument entity)
	{
		return new ResponseModel
		{
			Id = entity.Id,
			ProductId = entity.ProductId,
			DocumentUrl = entity.DocumentUrl,
			FileName = entity.FileName,
			Title = entity.Title,
			Type = (int)entity.Type,
			FileSizeBytes = entity.FileSizeBytes,
			MimeType = entity.MimeType,
			SortOrder = entity.SortOrder,
			CreatedAt = entity.CreatedAt,
			UpdatedAt = entity.UpdatedAt
		};
	}
}
