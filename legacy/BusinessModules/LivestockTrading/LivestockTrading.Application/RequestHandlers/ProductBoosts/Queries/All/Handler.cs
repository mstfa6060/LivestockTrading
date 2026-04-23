namespace LivestockTrading.Application.RequestHandlers.ProductBoosts.Queries.All;

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

		var (boosts, page) = await _dataAccessLayer.All(
			req.SellerId,
			req.Sorting,
			req.Filters,
			req.PageRequest,
			cancellationToken);

		var response = mapper.MapToResponse(boosts);
		return ArfBlocksResults.Success(response, page);
	}
}
