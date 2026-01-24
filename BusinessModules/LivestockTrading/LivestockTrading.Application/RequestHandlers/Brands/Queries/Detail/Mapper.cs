using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.Brands.Queries.Detail;

public class Mapper
{
	public ResponseModel MapToResponse(Brand entity)
	{
		return new ResponseModel
		{
			Id = entity.Id,
			Name = entity.Name,
			Slug = entity.Slug,
			Description = entity.Description,
			LogoUrl = entity.LogoUrl,
			Website = entity.Website,
			Email = entity.Email,
			Phone = entity.Phone,
			CountryCode = entity.CountryCode,
			IsActive = entity.IsActive,
			IsVerified = entity.IsVerified,
			ProductCount = entity.Products?.Count(p => !p.IsDeleted) ?? 0,
			CreatedAt = entity.CreatedAt,
			UpdatedAt = entity.UpdatedAt
		};
	}
}
