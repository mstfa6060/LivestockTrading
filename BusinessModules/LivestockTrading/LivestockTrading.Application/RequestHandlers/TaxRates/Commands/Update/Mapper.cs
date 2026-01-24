using LivestockTrading.Domain.Entities;
using LivestockTrading.Domain.Enums;

namespace LivestockTrading.Application.RequestHandlers.TaxRates.Commands.Update;

public class Mapper
{
	public void MapToEntity(RequestModel request, TaxRate entity)
	{
		entity.CountryCode = request.CountryCode;
		entity.StateCode = request.StateCode;
		entity.TaxName = request.TaxName;
		entity.Rate = request.Rate;
		entity.Type = (TaxType)request.Type;
		entity.CategoryId = request.CategoryId;
		entity.IsActive = request.IsActive;
		entity.ValidFrom = request.ValidFrom;
		entity.ValidUntil = request.ValidUntil;
		entity.UpdatedAt = DateTime.UtcNow;
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
			UpdatedAt = entity.UpdatedAt
		};
	}
}
