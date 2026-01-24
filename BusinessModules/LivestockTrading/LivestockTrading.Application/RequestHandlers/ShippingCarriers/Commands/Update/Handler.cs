namespace LivestockTrading.Application.RequestHandlers.ShippingCarriers.Commands.Update;

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

		var shippingCarrier = await _dataAccessLayer.GetShippingCarrierById(request.Id);

		if (shippingCarrier == null)
			throw new ArfBlocksValidationException(
				ErrorCodeGenerator.GetErrorCode(() => LivestockTradingDomainErrors.CommonErrors.IdNotValid));

		mapper.MapToEntity(request, shippingCarrier);

		await _dataAccessLayer.SaveChanges();

		var response = mapper.MapToResponse(shippingCarrier);
		return ArfBlocksResults.Success(response);
	}
}
