using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.PaymentMethods.Queries.All;

public class Mapper
{
	public List<ResponseModel> MapToResponse(List<PaymentMethod> items)
	{
		return items.Select(p => new ResponseModel
		{
			Id = p.Id,
			Name = p.Name,
			Code = p.Code,
			Description = p.Description,
			IconUrl = p.IconUrl,
			RequiresManualVerification = p.RequiresManualVerification,
			IsActive = p.IsActive,
			SupportedCountries = p.SupportedCountries,
			SupportedCurrencies = p.SupportedCurrencies,
			TransactionFeePercentage = p.TransactionFeePercentage,
			FixedTransactionFee = p.FixedTransactionFee,
			CreatedAt = p.CreatedAt
		}).ToList();
	}
}
