using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.Locations.Queries.Detail;

public class Mapper
{
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
			CreatedAt = entity.CreatedAt,
			UpdatedAt = entity.UpdatedAt
		};
	}
}
