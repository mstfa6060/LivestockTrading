using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.ProductDocuments.Commands.Update;

public class Mapper
{
	public void MapToEntity(RequestModel request, ProductDocument entity)
	{
		entity.DocumentUrl = request.DocumentUrl;
		entity.FileName = request.FileName;
		entity.Title = request.Title;
		entity.Type = (DocumentType)request.Type;
		entity.FileSizeBytes = request.FileSizeBytes;
		entity.MimeType = request.MimeType;
		entity.SortOrder = request.SortOrder;
		entity.UpdatedAt = DateTime.UtcNow;
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
			UpdatedAt = entity.UpdatedAt
		};
	}
}
