namespace LivestockTrading.Application.RequestHandlers.Preferences.Queries.My;

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

		var preferences = await _dataAccessLayer.GetByUserId(req.UserId, cancellationToken);

		if (preferences == null)
		{
			// Create default preferences
			preferences = new LivestockTrading.Domain.Entities.UserPreferences
			{
				Id = Guid.NewGuid(),
				UserId = req.UserId,
				CreatedAt = DateTime.UtcNow,
				UpdatedAt = DateTime.UtcNow
			};
			await _dataAccessLayer.Add(preferences, cancellationToken);
		}

		var response = mapper.MapToResponse(preferences);
		return ArfBlocksResults.Success(response);
	}
}
