namespace LivestockTrading.Application.RequestHandlers.Preferences.Commands.Update;

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

		var preferences = await _dataAccessLayer.GetByUserId(request.UserId, cancellationToken);

		if (preferences == null)
		{
			// Create new preferences with provided values
			preferences = mapper.MapToEntity(request);
			await _dataAccessLayer.Add(preferences, cancellationToken);
		}
		else
		{
			// Update existing
			mapper.MapToEntity(request, preferences);
			await _dataAccessLayer.SaveChanges(cancellationToken);
		}

		var response = mapper.MapToResponse(preferences);
		return ArfBlocksResults.Success(response);
	}
}
