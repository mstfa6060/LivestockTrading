namespace LivestockTrading.Application.RequestHandlers.ContactForms.Commands.Create;

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

		var entity = mapper.MapToEntity(request);

		await _dataAccessLayer.Add(entity);

		var response = new ResponseModel { Id = entity.Id, Success = true };
		return ArfBlocksResults.Success(response);
	}
}
