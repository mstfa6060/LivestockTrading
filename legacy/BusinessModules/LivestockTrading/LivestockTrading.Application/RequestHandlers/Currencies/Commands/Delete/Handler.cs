namespace LivestockTrading.Application.RequestHandlers.Currencies.Commands.Delete;

public class Handler : IRequestHandler
{
	private readonly DataAccess _dataAccessLayer;

	public Handler(ArfBlocksDependencyProvider dependencyProvider, object dataAccess)
	{
		_dataAccessLayer = (DataAccess)dataAccess;
	}

	public async Task<ArfBlocksRequestResult> Handle(IRequestModel payload, EndpointContext context, CancellationToken cancellationToken)
	{
		var request = (RequestModel)payload;

		var currency = await _dataAccessLayer.GetCurrencyById(request.Id);

		if (currency == null)
			throw new ArfBlocksValidationException(
				ErrorCodeGenerator.GetErrorCode(() => LivestockTradingDomainErrors.CommonErrors.IdNotValid));

		currency.IsDeleted = true;
		currency.DeletedAt = DateTime.UtcNow;

		await _dataAccessLayer.SaveChanges();

		return ArfBlocksResults.Success(new ResponseModel { Success = true });
	}
}
