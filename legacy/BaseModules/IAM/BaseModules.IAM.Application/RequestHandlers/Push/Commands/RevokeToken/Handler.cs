namespace BaseModules.Notification.Application.RequestHandlers.Push.Commands.RevokeToken;

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

		await _dataAccessLayer.RevokeToken(request.UserId, request.DeviceId);

		return ArfBlocksResults.Success(new ResponseModel { Success = true });
	}
}
