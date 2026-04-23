using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.Farms.Queries.Detail;

public class Mapper
{
	public ResponseModel MapToResponse(Farm entity)
	{
		return new ResponseModel
		{
			Id = entity.Id,
			Name = entity.Name,
			Description = entity.Description,
			RegistrationNumber = entity.RegistrationNumber,
			SellerId = entity.SellerId,
			SellerBusinessName = entity.Seller?.BusinessName,
			LocationId = entity.LocationId,
			LocationName = entity.Location?.Name,
			Type = (int)entity.Type,
			TotalAreaHectares = entity.TotalAreaHectares,
			CultivatedAreaHectares = entity.CultivatedAreaHectares,
			Certifications = entity.Certifications,
			IsOrganic = entity.IsOrganic,
			ImageUrls = entity.ImageUrls,
			VideoUrl = entity.VideoUrl,
			IsActive = entity.IsActive,
			IsVerified = entity.IsVerified,
			CreatedAt = entity.CreatedAt,
			UpdatedAt = entity.UpdatedAt
		};
	}
}
