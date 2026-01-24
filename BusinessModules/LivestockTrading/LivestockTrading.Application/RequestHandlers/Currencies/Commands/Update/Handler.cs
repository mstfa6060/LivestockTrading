namespace LivestockTrading.Application.RequestHandlers.Currencies.Commands.Update;

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
		var mapper = new Mapper();

		var currency = await _dataAccessLayer.GetCurrencyById(request.Id);

		if (currency == null)
			throw new ArfBlocksValidationException(
				ErrorCodeGenerator.GetErrorCode(() => LivestockTradingDomainErrors.CommonErrors.IdNotValid));

		mapper.MapToEntity(request, currency);

		await _dataAccessLayer.SaveChanges();

		var response = mapper.MapToResponse(currency);
		return ArfBlocksResults.Success(response);
	}
}
