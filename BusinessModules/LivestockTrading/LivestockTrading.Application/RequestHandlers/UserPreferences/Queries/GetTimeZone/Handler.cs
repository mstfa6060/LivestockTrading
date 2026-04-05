namespace LivestockTrading.Application.RequestHandlers.UserPreferences.Queries.GetTimeZone;

public class Handler : IRequestHandler
{
	private readonly DataAccess _dataAccessLayer;

	public Handler(ArfBlocksDependencyProvider dependencyProvider, object dataAccess)
	{
		_dataAccessLayer = (DataAccess)dataAccess;
	}

	public async Task<ArfBlocksRequestResult> Handle(IRequestModel payload, EndpointContext context, CancellationToken cancellationToken)
	{
		var request = (RequestModel)payload;
		var mapper = new Mapper();

		var timeZone = await _dataAccessLayer.GetTimeZoneByUserId(request.UserId, cancellationToken);
		var response = mapper.MapToResponse(request.UserId, timeZone);

		return ArfBlocksResults.Success(response);
	}
}
