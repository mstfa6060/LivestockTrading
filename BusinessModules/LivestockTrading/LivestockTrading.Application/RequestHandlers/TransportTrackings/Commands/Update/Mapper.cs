using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.TransportTrackings.Commands.Update;

public class Mapper
{
	public void MapToEntity(RequestModel request, TransportTracking entity)
	{
		entity.TransportRequestId = request.TransportRequestId;
		entity.Latitude = request.Latitude;
		entity.Longitude = request.Longitude;
		entity.LocationDescription = request.LocationDescription;
		entity.Status = (TrackingStatus)request.Status;
		entity.StatusDescription = request.StatusDescription;
		entity.RecordedAt = request.RecordedAt;
		entity.Notes = request.Notes;
		entity.PhotoUrls = request.PhotoUrls;
		entity.UpdatedAt = DateTime.UtcNow;
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
			CreatedAt = entity.CreatedAt,
			UpdatedAt = entity.UpdatedAt
		};
	}
}
