namespace BaseModules.IAM.Application.RequestHandlers.Users.Queries.All;

/// <summary>
/// Tüm Kullanıcıları Listele
/// Bu endpoint, sistemdeki tüm kullanıcıların listesini döner.
/// Filtreleme ve sayfalama desteği sunar.
/// </summary>
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
		var requestPayload = (RequestModel)payload;

		var (users, userRoles, page) = await _dataAccessLayer.All(
			requestPayload.Sorting,
			requestPayload.Filters,
			requestPayload.PageRequest);

		var response = mapper.MapToResponse(users, userRoles);

		return ArfBlocksResults.Success(response, page);
	}
}
