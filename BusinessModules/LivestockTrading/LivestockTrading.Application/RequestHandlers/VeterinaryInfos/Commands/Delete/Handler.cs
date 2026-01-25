namespace LivestockTrading.Application.RequestHandlers.VeterinaryInfos.Commands.Delete;

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

		var entity = (await _dataAccessLayer.GetById(request.Id)).EnsureExists();

		entity.IsDeleted = true;
		entity.DeletedAt = DateTime.UtcNow;

		await _dataAccessLayer.SaveChanges();

		return ArfBlocksResults.Success(new ResponseModel { Success = true });
	}
}
