using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.TransportTrackings.Queries.Pick;

public class Mapper
{
	public List<ResponseModel> MapToResponse(List<TransportTracking> items)
	{
		return items.Select(e => new ResponseModel
		{
			Id = e.Id,
			LocationDescription = e.LocationDescription,
			RecordedAt = e.RecordedAt
		}).ToList();
	}
}
