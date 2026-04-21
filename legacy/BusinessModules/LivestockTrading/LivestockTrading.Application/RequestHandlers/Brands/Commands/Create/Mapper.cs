using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.Brands.Commands.Create;

public class Mapper
{
	public Brand MapToEntity(RequestModel request)
	{
		return new Brand
		{
			Id = Guid.NewGuid(),
			Name = request.Name,
			Slug = request.Slug,
			Description = request.Description,
			LogoUrl = request.LogoUrl,
			Website = request.Website,
			Email = request.Email,
			Phone = request.Phone,
			CountryCode = request.CountryCode,
			IsActive = request.IsActive,
			IsVerified = false,
			CreatedAt = DateTime.UtcNow
		};
	}

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
			CreatedAt = entity.CreatedAt
		};
	}
}
