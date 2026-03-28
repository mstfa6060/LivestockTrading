using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.Locations.Commands.Update;

public class Mapper
{
	public void MapToEntity(RequestModel request, Location entity)
	{
		entity.Name = request.Name;
		entity.AddressLine1 = request.AddressLine1;
		entity.AddressLine2 = request.AddressLine2;
		entity.City = request.City;
		entity.State = request.State;
		entity.PostalCode = request.PostalCode;
		entity.CountryCode = request.CountryCode;
		entity.Latitude = request.Latitude;
		entity.Longitude = request.Longitude;
		entity.Phone = request.Phone;
		entity.Email = request.Email;
		entity.Type = (LocationType)request.Type;
		entity.IsActive = request.IsActive;
		entity.UserId = request.UserId;
		entity.DistrictId = request.DistrictId;
		entity.UpdatedAt = DateTime.UtcNow;
	}

	public ResponseModel MapToResponse(Location entity)
	{
		return new ResponseModel
		{
			Id = entity.Id,
			Name = entity.Name,
			AddressLine1 = entity.AddressLine1,
			AddressLine2 = entity.AddressLine2,
			City = entity.City,
			State = entity.State,
			PostalCode = entity.PostalCode,
			CountryCode = entity.CountryCode,
			Latitude = entity.Latitude,
			Longitude = entity.Longitude,
			Phone = entity.Phone,
			Email = entity.Email,
			Type = (int)entity.Type,
			IsActive = entity.IsActive,
			UserId = entity.UserId,
			DistrictId = entity.DistrictId,
			UpdatedAt = entity.UpdatedAt
		};
	}
}
