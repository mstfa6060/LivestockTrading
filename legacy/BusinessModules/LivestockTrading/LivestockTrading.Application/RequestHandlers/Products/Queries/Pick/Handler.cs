namespace LivestockTrading.Application.RequestHandlers.Products.Queries.Pick;

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

		var products = await _dataAccessLayer.Pick(
			req.SelectedIds,
			req.Keyword,
			req.Limit,
			cancellationToken);

		var response = mapper.MapToResponse(products);

		return ArfBlocksResults.Success(response);
	}
}
