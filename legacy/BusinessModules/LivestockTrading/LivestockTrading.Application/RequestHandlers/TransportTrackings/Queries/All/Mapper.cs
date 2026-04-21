using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.TransportTrackings.Queries.All;

public class Mapper
{
	public List<ResponseModel> MapToResponse(List<TransportTracking> items)
	{
		return items.Select(e => new ResponseModel
		{
			Id = e.Id,
			TransportRequestId = e.TransportRequestId,
			Latitude = e.Latitude,
			Longitude = e.Longitude,
			LocationDescription = e.LocationDescription,
			Status = (int)e.Status,
			StatusDescription = e.StatusDescription,
			RecordedAt = e.RecordedAt,
			Notes = e.Notes,
			PhotoUrls = e.PhotoUrls,
			CreatedAt = e.CreatedAt
		}).ToList();
	}
}
