namespace LivestockTrading.Application.RequestHandlers.Categories.Queries.All;

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

		var (categories, productCounts, page) = await _dataAccessLayer.All(
			req.Sorting,
			req.Filters,
			req.PageRequest,
			cancellationToken);

		var response = mapper.MapToResponse(categories, productCounts, req.LanguageCode);

		return ArfBlocksResults.Success(response, page);
	}
}
