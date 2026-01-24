using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.Locations.Commands.Create;

public class Mapper
{
	public Location MapToEntity(RequestModel request)
	{
		return new Location
		{
			Id = Guid.NewGuid(),
			Name = request.Name,
			AddressLine1 = request.AddressLine1,
			AddressLine2 = request.AddressLine2,
			City = request.City,
			State = request.State,
			PostalCode = request.PostalCode,
			CountryCode = request.CountryCode,
			Latitude = request.Latitude,
			Longitude = request.Longitude,
			Phone = request.Phone,
			Email = request.Email,
			Type = (LocationType)request.Type,
			IsActive = request.IsActive,
			UserId = request.UserId,
			CreatedAt = DateTime.UtcNow
		};
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
			CreatedAt = entity.CreatedAt
		};
	}
}
