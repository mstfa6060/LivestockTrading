using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.PaymentMethods.Queries.Detail;

public class Mapper
{
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
			CreatedAt = entity.CreatedAt,
			UpdatedAt = entity.UpdatedAt
		};
	}
}
