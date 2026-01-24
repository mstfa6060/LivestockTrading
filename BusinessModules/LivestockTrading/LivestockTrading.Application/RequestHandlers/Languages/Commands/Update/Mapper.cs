using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.Languages.Commands.Update;

public class Mapper
{
	public void MapToEntity(RequestModel request, Language entity)
	{
		entity.Code = request.Code;
		entity.Name = request.Name;
		entity.NativeName = request.NativeName;
		entity.IsRightToLeft = request.IsRightToLeft;
		entity.IsActive = request.IsActive;
		entity.IsDefault = request.IsDefault;
		entity.SortOrder = request.SortOrder;
		entity.FlagIconUrl = request.FlagIconUrl;
		entity.UpdatedAt = DateTime.UtcNow;
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
			UpdatedAt = entity.UpdatedAt
		};
	}
}
