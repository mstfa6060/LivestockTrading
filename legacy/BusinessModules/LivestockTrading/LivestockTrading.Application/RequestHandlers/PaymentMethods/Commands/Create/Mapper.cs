using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.PaymentMethods.Commands.Create;

public class Mapper
{
	public PaymentMethod MapToEntity(RequestModel request)
	{
		return new PaymentMethod
		{
			Id = Guid.NewGuid(),
			Name = request.Name,
			Code = request.Code,
			Description = request.Description,
			IconUrl = request.IconUrl,
			RequiresManualVerification = request.RequiresManualVerification,
			IsActive = request.IsActive,
			SupportedCountries = request.SupportedCountries,
			SupportedCurrencies = request.SupportedCurrencies,
			TransactionFeePercentage = request.TransactionFeePercentage,
			FixedTransactionFee = request.FixedTransactionFee,
			CreatedAt = DateTime.UtcNow
		};
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
			CreatedAt = entity.CreatedAt
		};
	}
}
