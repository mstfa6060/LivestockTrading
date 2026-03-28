using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.Locations.Queries.All;

public class Mapper
{
	public List<ResponseModel> MapToResponse(List<Location> locations)
	{
		return locations.Select(l => new ResponseModel
		{
			Id = l.Id,
			Name = l.Name,
			AddressLine1 = l.AddressLine1,
			AddressLine2 = l.AddressLine2,
			City = l.City,
			State = l.State,
			PostalCode = l.PostalCode,
			CountryCode = l.CountryCode,
			Latitude = l.Latitude,
			Longitude = l.Longitude,
			Phone = l.Phone,
			Email = l.Email,
			Type = (int)l.Type,
			IsActive = l.IsActive,
			UserId = l.UserId,
			DistrictId = l.DistrictId,
			CreatedAt = l.CreatedAt
		}).ToList();
	}
}
