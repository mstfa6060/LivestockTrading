using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.TransportTrackings.Queries.Detail;

public class Mapper
{
	public ResponseModel MapToResponse(TransportTracking entity)
	{
		return new ResponseModel
		{
			Id = entity.Id,
			TransportRequestId = entity.TransportRequestId,
			Latitude = entity.Latitude,
			Longitude = entity.Longitude,
			LocationDescription = entity.LocationDescription,
			Status = (int)entity.Status,
			StatusDescription = entity.StatusDescription,
			RecordedAt = entity.RecordedAt,
			Notes = entity.Notes,
			PhotoUrls = entity.PhotoUrls,
			CreatedAt = entity.CreatedAt
		};
	}
}
