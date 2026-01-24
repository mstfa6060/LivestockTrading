using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.Brands.Queries.All;

public class Mapper
{
	public List<ResponseModel> MapToResponse(List<Brand> brands)
	{
		return brands.Select(b => new ResponseModel
		{
			Id = b.Id,
			Name = b.Name,
			Slug = b.Slug,
			Description = b.Description,
			LogoUrl = b.LogoUrl,
			Website = b.Website,
			Email = b.Email,
			Phone = b.Phone,
			CountryCode = b.CountryCode,
			IsActive = b.IsActive,
			IsVerified = b.IsVerified,
			ProductCount = b.Products?.Count(p => !p.IsDeleted) ?? 0,
			CreatedAt = b.CreatedAt
		}).ToList();
	}
}
