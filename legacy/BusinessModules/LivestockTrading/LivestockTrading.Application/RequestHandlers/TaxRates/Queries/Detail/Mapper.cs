using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.TaxRates.Queries.Detail;

public class Mapper
{
	public ResponseModel MapToResponse(TaxRate entity)
	{
		return new ResponseModel
		{
			Id = entity.Id,
			CountryCode = entity.CountryCode,
			StateCode = entity.StateCode,
			TaxName = entity.TaxName,
			Rate = entity.Rate,
			Type = (int)entity.Type,
			CategoryId = entity.CategoryId,
			IsActive = entity.IsActive,
			ValidFrom = entity.ValidFrom,
			ValidUntil = entity.ValidUntil,
			CreatedAt = entity.CreatedAt,
			UpdatedAt = entity.UpdatedAt
		};
	}
}
