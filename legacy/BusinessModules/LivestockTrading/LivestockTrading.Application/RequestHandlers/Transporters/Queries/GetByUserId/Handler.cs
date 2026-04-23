namespace LivestockTrading.Application.RequestHandlers.Transporters.Queries.GetByUserId;

public class Handler : IRequestHandler
{
	private readonly DataAccess _dataAccessLayer;

	public Handler(ArfBlocksDependencyProvider dependencyProvider, object dataAccess)
	{
		_dataAccessLayer = (DataAccess)dataAccess;
	}

	public async Task<ArfBlocksRequestResult> Handle(IRequestModel payload, EndpointContext context, CancellationToken cancellationToken)
	{
		var mapper = new Mapper();
		var req = (RequestModel)payload;

		var transporter = await _dataAccessLayer.GetByUserId(req.UserId, cancellationToken);
		if (transporter == null)
			throw new ArfBlocksValidationException(
				ErrorCodeGenerator.GetErrorCode(() => LivestockTradingDomainErrors.TransporterErrors.TransporterNotFound));

		return ArfBlocksResults.Success(mapper.MapToResponse(transporter));
	}
}
