namespace LivestockTrading.Application.RequestHandlers.Conversations.Queries.All;

public class Handler : IRequestHandler
{
	private readonly DataAccess _dataAccessLayer;
	private readonly CurrentUserService _currentUserService;

	public Handler(ArfBlocksDependencyProvider dependencyProvider, object dataAccess)
	{
		_dataAccessLayer = (DataAccess)dataAccess;
		_currentUserService = dependencyProvider.GetInstance<CurrentUserService>();
	}

	public async Task<ArfBlocksRequestResult> Handle(IRequestModel payload, EndpointContext context, CancellationToken cancellationToken)
	{
		var mapper = new Mapper();
		var req = (RequestModel)payload;
		var currentUserId = _currentUserService.GetCurrentUserId();

		var (conversations, extras, page) = await _dataAccessLayer.All(
			currentUserId,
			req.Sorting,
			req.Filters,
			req.PageRequest,
			cancellationToken);

		var response = mapper.MapToResponse(conversations, extras);

		return ArfBlocksResults.Success(response, page);
	}
}
