namespace LivestockTrading.Application.RequestHandlers.TransportRequests.Commands.Update;

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

		var transportRequest = await _dataAccessLayer.GetTransportRequestById(request.Id);

		if (transportRequest == null)
			throw new ArfBlocksValidationException(
				ErrorCodeGenerator.GetErrorCode(() => LivestockTradingDomainErrors.CommonErrors.IdNotValid));

		mapper.MapToEntity(request, transportRequest);

		await _dataAccessLayer.SaveChanges();

		var response = mapper.MapToResponse(transportRequest);
		return ArfBlocksResults.Success(response);
	}
}
