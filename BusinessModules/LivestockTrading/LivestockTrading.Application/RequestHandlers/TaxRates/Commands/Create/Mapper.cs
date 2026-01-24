using LivestockTrading.Domain.Entities;
using LivestockTrading.Domain.Enums;

namespace LivestockTrading.Application.RequestHandlers.TaxRates.Commands.Create;

public class Mapper
{
	public TaxRate MapToEntity(RequestModel request)
	{
		return new TaxRate
		{
			Id = Guid.NewGuid(),
			CountryCode = request.CountryCode,
			StateCode = request.StateCode,
			TaxName = request.TaxName,
			Rate = request.Rate,
			Type = (TaxType)request.Type,
			CategoryId = request.CategoryId,
			IsActive = request.IsActive,
			ValidFrom = request.ValidFrom,
			ValidUntil = request.ValidUntil,
			CreatedAt = DateTime.UtcNow
		};
	}

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
			CreatedAt = entity.CreatedAt
		};
	}
}
