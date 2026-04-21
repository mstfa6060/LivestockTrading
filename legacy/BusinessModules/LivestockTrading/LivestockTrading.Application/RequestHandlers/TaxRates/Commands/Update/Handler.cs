namespace LivestockTrading.Application.RequestHandlers.TaxRates.Commands.Update;

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

		var taxRate = await _dataAccessLayer.GetTaxRateById(request.Id);

		if (taxRate == null)
			throw new ArfBlocksValidationException(
				ErrorCodeGenerator.GetErrorCode(() => LivestockTradingDomainErrors.CommonErrors.IdNotValid));

		mapper.MapToEntity(request, taxRate);

		await _dataAccessLayer.SaveChanges();

		var response = mapper.MapToResponse(taxRate);
		return ArfBlocksResults.Success(response);
	}
}
