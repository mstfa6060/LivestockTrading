using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.PaymentMethods.Commands.Update;

public class Mapper
{
	public void MapToEntity(RequestModel request, PaymentMethod entity)
	{
		entity.Name = request.Name;
		entity.Code = request.Code;
		entity.Description = request.Description;
		entity.IconUrl = request.IconUrl;
		entity.RequiresManualVerification = request.RequiresManualVerification;
		entity.IsActive = request.IsActive;
		entity.SupportedCountries = request.SupportedCountries;
		entity.SupportedCurrencies = request.SupportedCurrencies;
		entity.TransactionFeePercentage = request.TransactionFeePercentage;
		entity.FixedTransactionFee = request.FixedTransactionFee;
		entity.UpdatedAt = DateTime.UtcNow;
	}

	public ResponseModel MapToResponse(PaymentMethod entity)
	{
		return new ResponseModel
		{
			Id = entity.Id,
			Name = entity.Name,
			Code = entity.Code,
			Description = entity.Description,
			IconUrl = entity.IconUrl,
			RequiresManualVerification = entity.RequiresManualVerification,
			IsActive = entity.IsActive,
			SupportedCountries = entity.SupportedCountries,
			SupportedCurrencies = entity.SupportedCurrencies,
			TransactionFeePercentage = entity.TransactionFeePercentage,
			FixedTransactionFee = entity.FixedTransactionFee,
			UpdatedAt = entity.UpdatedAt
		};
	}
}
