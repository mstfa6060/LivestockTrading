using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.Farms.Commands.Create;

public class Mapper
{
	public Farm MapToEntity(RequestModel request)
	{
		return new Farm
		{
			Id = Guid.NewGuid(),
			Name = request.Name,
			Description = request.Description,
			RegistrationNumber = request.RegistrationNumber,
			SellerId = request.SellerId,
			LocationId = request.LocationId,
			Type = (FarmType)request.Type,
			TotalAreaHectares = request.TotalAreaHectares,
			CultivatedAreaHectares = request.CultivatedAreaHectares,
			Certifications = request.Certifications,
			IsOrganic = request.IsOrganic,
			ImageUrls = request.ImageUrls,
			VideoUrl = request.VideoUrl,
			IsActive = request.IsActive,
			IsVerified = false,
			CreatedAt = DateTime.UtcNow
		};
	}

	public ResponseModel MapToResponse(Farm entity)
	{
		return new ResponseModel
		{
			Id = entity.Id,
			Name = entity.Name,
			Description = entity.Description,
			RegistrationNumber = entity.RegistrationNumber,
			SellerId = entity.SellerId,
			LocationId = entity.LocationId,
			Type = (int)entity.Type,
			TotalAreaHectares = entity.TotalAreaHectares,
			CultivatedAreaHectares = entity.CultivatedAreaHectares,
			Certifications = entity.Certifications,
			IsOrganic = entity.IsOrganic,
			ImageUrls = entity.ImageUrls,
			VideoUrl = entity.VideoUrl,
			IsActive = entity.IsActive,
			IsVerified = entity.IsVerified,
			CreatedAt = entity.CreatedAt
		};
	}
}
