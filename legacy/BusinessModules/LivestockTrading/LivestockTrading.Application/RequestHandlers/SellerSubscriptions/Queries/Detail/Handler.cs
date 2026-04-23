namespace LivestockTrading.Application.RequestHandlers.SellerSubscriptions.Queries.Detail;

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

		var subscription = await _dataAccessLayer.GetBySellerId(req.SellerId, cancellationToken);
		if (subscription == null)
			return ArfBlocksResults.Success(new ResponseModel { SellerId = req.SellerId, Status = -1 });

		var activeListings = await _dataAccessLayer.GetActiveListingCount(req.SellerId, cancellationToken);
		var response = mapper.MapToResponse(subscription, activeListings);
		return ArfBlocksResults.Success(response);
	}
}
