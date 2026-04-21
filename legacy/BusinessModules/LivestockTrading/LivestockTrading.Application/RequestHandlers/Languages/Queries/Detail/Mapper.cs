using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.Languages.Queries.Detail;

public class Mapper
{
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
			CreatedAt = entity.CreatedAt,
			UpdatedAt = entity.UpdatedAt
		};
	}
}
