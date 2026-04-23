namespace LivestockTrading.Application.RequestHandlers.Currencies.Queries.Pick;

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

		var currencies = await _dataAccessLayer.Pick(
			req.SelectedIds,
			req.Keyword,
			req.Limit,
			cancellationToken);

		var response = mapper.MapToResponse(currencies);

		return ArfBlocksResults.Success(response);
	}
}
