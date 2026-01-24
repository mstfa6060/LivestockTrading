using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.ProductDocuments.Commands.Create;

public class Mapper
{
	public ProductDocument MapToEntity(RequestModel request)
	{
		return new ProductDocument
		{
			Id = Guid.NewGuid(),
			ProductId = request.ProductId,
			DocumentUrl = request.DocumentUrl,
			FileName = request.FileName,
			Title = request.Title,
			Type = (DocumentType)request.Type,
			FileSizeBytes = request.FileSizeBytes,
			MimeType = request.MimeType,
			SortOrder = request.SortOrder,
			CreatedAt = DateTime.UtcNow
		};
	}

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
			CreatedAt = entity.CreatedAt
		};
	}
}
