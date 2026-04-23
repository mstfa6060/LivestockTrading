using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.TransportTrackings.Commands.Create;

public class Mapper
{
	public TransportTracking MapToEntity(RequestModel request)
	{
		return new TransportTracking
		{
			Id = Guid.NewGuid(),
			TransportRequestId = request.TransportRequestId,
			Latitude = request.Latitude,
			Longitude = request.Longitude,
			LocationDescription = request.LocationDescription,
			Status = (TrackingStatus)request.Status,
			StatusDescription = request.StatusDescription,
			RecordedAt = request.RecordedAt ?? DateTime.UtcNow,
			Notes = request.Notes,
			PhotoUrls = request.PhotoUrls,
			CreatedAt = DateTime.UtcNow
		};
	}

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
