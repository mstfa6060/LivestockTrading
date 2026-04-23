namespace LivestockTrading.Application.RequestHandlers.Currencies.Commands.UpdateRates;

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
		var now = DateTime.UtcNow;

		// DB'deki aktif para birimlerini al
		var currencies = await _dataAccessLayer.GetActiveCurrencies(cancellationToken);
		var currencyMap = currencies.ToDictionary(c => c.Code, c => c, StringComparer.OrdinalIgnoreCase);

		var updatedCount = 0;

		foreach (var rateItem in req.Rates)
		{
			if (currencyMap.TryGetValue(rateItem.Code, out var currency))
			{
				currency.ExchangeRateToUSD = rateItem.ExchangeRateToUSD;
				currency.LastUpdated = now;
				currency.UpdatedAt = now;
				updatedCount++;
			}
		}

		if (updatedCount > 0)
			await _dataAccessLayer.SaveChanges(cancellationToken);

		return ArfBlocksResults.Success(new ResponseModel
		{
			UpdatedCount = updatedCount,
			UpdatedAt = now
		});
	}
}
