using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.Brands.Commands.Update;

public class Mapper
{
	public void MapToEntity(RequestModel request, Brand entity)
	{
		entity.Name = request.Name;
		entity.Slug = request.Slug;
		entity.Description = request.Description;
		entity.LogoUrl = request.LogoUrl;
		entity.Website = request.Website;
		entity.Email = request.Email;
		entity.Phone = request.Phone;
		entity.CountryCode = request.CountryCode;
		entity.IsActive = request.IsActive;
		entity.IsVerified = request.IsVerified;
		entity.UpdatedAt = DateTime.UtcNow;
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
			UpdatedAt = entity.UpdatedAt
		};
	}
}
