using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.Farms.Commands.Update;

public class Mapper
{
	public void MapToEntity(RequestModel request, Farm entity)
	{
		entity.Name = request.Name;
		entity.Description = request.Description;
		entity.RegistrationNumber = request.RegistrationNumber;
		entity.SellerId = request.SellerId;
		entity.LocationId = request.LocationId;
		entity.Type = (FarmType)request.Type;
		entity.TotalAreaHectares = request.TotalAreaHectares;
		entity.CultivatedAreaHectares = request.CultivatedAreaHectares;
		entity.Certifications = request.Certifications;
		entity.IsOrganic = request.IsOrganic;
		entity.ImageUrls = request.ImageUrls;
		entity.VideoUrl = request.VideoUrl;
		entity.IsActive = request.IsActive;
		entity.IsVerified = request.IsVerified;
		entity.UpdatedAt = DateTime.UtcNow;
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
			UpdatedAt = entity.UpdatedAt
		};
	}
}
