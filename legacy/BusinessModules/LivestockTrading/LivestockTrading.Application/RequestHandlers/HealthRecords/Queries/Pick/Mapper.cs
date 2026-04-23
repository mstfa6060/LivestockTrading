using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.HealthRecords.Queries.Pick;

public class Mapper
{
	public List<ResponseModel> MapToResponse(List<HealthRecord> items)
	{
		return items.Select(e => new ResponseModel
		{
			Id = e.Id,
			RecordType = e.RecordType,
			RecordDate = e.RecordDate
		}).ToList();
	}
}
