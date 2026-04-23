namespace LivestockTrading.Application.RequestHandlers.VeterinaryInfos.Commands.Update;

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

		var entity = (await _dataAccessLayer.GetById(request.Id)).EnsureExists();

		mapper.MapToEntity(request, entity);

		await _dataAccessLayer.SaveChanges();

		var response = mapper.MapToResponse(entity);
		return ArfBlocksResults.Success(response);
	}
}
