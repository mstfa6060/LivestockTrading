namespace LivestockTrading.Application.RequestHandlers.Sellers.Queries.DetailByUserId;

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

		var seller = await _dataAccessLayer.GetByUserId(req.UserId, cancellationToken);
		if (seller == null)
			throw new ArfBlocksValidationException(
				ErrorCodeGenerator.GetErrorCode(() => LivestockTradingDomainErrors.SellerErrors.SellerNotFound));

		var response = mapper.MapToResponse(seller);

		return ArfBlocksResults.Success(response);
	}
}
