namespace LivestockTrading.Application.RequestHandlers.SearchHistories.Queries.Detail;

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

		var entity = (await _dataAccessLayer.GetById(req.Id, cancellationToken)).EnsureExists();

		var response = mapper.MapToResponse(entity);

		return ArfBlocksResults.Success(response);
	}
}
