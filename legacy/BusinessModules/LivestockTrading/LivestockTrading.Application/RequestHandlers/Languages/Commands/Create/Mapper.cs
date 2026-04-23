using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.Languages.Commands.Create;

public class Mapper
{
	public Language MapToEntity(RequestModel request)
	{
		return new Language
		{
			Id = Guid.NewGuid(),
			Code = request.Code,
			Name = request.Name,
			NativeName = request.NativeName,
			IsRightToLeft = request.IsRightToLeft,
			IsActive = request.IsActive,
			IsDefault = request.IsDefault,
			SortOrder = request.SortOrder,
			FlagIconUrl = request.FlagIconUrl,
			CreatedAt = DateTime.UtcNow
		};
	}

	public ResponseModel MapToResponse(Language entity)
	{
		return new ResponseModel
		{
			Id = entity.Id,
			Code = entity.Code,
			Name = entity.Name,
			NativeName = entity.NativeName,
			IsRightToLeft = entity.IsRightToLeft,
			IsActive = entity.IsActive,
			IsDefault = entity.IsDefault,
			SortOrder = entity.SortOrder,
			FlagIconUrl = entity.FlagIconUrl,
			CreatedAt = entity.CreatedAt
		};
	}
}
