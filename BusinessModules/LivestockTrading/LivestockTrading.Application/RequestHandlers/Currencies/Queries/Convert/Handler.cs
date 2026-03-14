namespace LivestockTrading.Application.RequestHandlers.Currencies.Queries.Convert;

public class Handler : IRequestHandler
{
	private readonly DataAccess _dataAccessLayer;

	public Handler(ArfBlocksDependencyProvider dependencyProvider, object dataAccess)
	{
		_dataAccessLayer = (DataAccess)dataAccess;
	}

	public async Task<ArfBlocksRequestResult> Handle(IRequestModel payload, EndpointContext context, CancellationToken cancellationToken)
	{
		var req = (RequestModel)payload;

		var fromCurrency = await _dataAccessLayer.GetByCode(req.FromCurrencyCode.ToUpperInvariant(), cancellationToken);
		if (fromCurrency == null)
			throw new ArfBlocksValidationException(
				ErrorCodeGenerator.GetErrorCode(() => LivestockTradingDomainErrors.CurrencyErrors.CurrencyFromNotFound));

		var toCurrency = await _dataAccessLayer.GetByCode(req.ToCurrencyCode.ToUpperInvariant(), cancellationToken);
		if (toCurrency == null)
			throw new ArfBlocksValidationException(
				ErrorCodeGenerator.GetErrorCode(() => LivestockTradingDomainErrors.CurrencyErrors.CurrencyToNotFound));

		// Dönüşüm: Amount (FromCurrency) → USD → ToCurrency
		// fromCurrency.ExchangeRateToUSD = 1 USD kaç FromCurrency
		// toCurrency.ExchangeRateToUSD = 1 USD kaç ToCurrency
		// Formül: convertedAmount = (amount / fromRate) * toRate
		var amountInUsd = req.Amount / fromCurrency.ExchangeRateToUSD;
		var convertedAmount = Math.Round(amountInUsd * toCurrency.ExchangeRateToUSD, 2);

		var exchangeRate = Math.Round(toCurrency.ExchangeRateToUSD / fromCurrency.ExchangeRateToUSD, 6);

		var lastUpdated = fromCurrency.LastUpdated < toCurrency.LastUpdated
			? fromCurrency.LastUpdated
			: toCurrency.LastUpdated;

		var response = new ResponseModel
		{
			OriginalAmount = req.Amount,
			ConvertedAmount = convertedAmount,
			FromCurrencyCode = fromCurrency.Code,
			ToCurrencyCode = toCurrency.Code,
			ExchangeRate = exchangeRate,
			FromSymbol = fromCurrency.Symbol,
			ToSymbol = toCurrency.Symbol,
			LastUpdated = lastUpdated
		};

		return ArfBlocksResults.Success(response);
	}
}
