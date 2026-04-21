namespace LivestockTrading.Application.RequestHandlers.ProductViewHistories.Queries.Pick;

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

		var items = await _dataAccessLayer.Pick(
			req.SelectedIds,
			req.Keyword,
			req.Limit,
			cancellationToken);

		var response = mapper.MapToResponse(items);

		return ArfBlocksResults.Success(response);
	}
}
