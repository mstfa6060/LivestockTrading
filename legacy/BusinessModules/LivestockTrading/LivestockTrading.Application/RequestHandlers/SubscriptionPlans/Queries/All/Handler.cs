namespace LivestockTrading.Application.RequestHandlers.SubscriptionPlans.Queries.All;

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

		var (plans, page) = await _dataAccessLayer.All(
			req.LanguageCode,
			req.TargetType,
			req.Sorting,
			req.Filters,
			req.PageRequest,
			cancellationToken);

		var response = mapper.MapToResponse(plans, req.LanguageCode);

		return ArfBlocksResults.Success(response, page);
	}
}
