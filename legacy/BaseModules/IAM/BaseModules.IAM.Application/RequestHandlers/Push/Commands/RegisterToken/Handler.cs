namespace BaseModules.Notification.Application.RequestHandlers.Push.Commands.RegisterToken;

/// <summary>
/// Push Notification Token Kaydetme
/// Bu endpoint, kullanıcının mobil cihaz push notification token'ını kaydeder.
/// Bildirim gönderimi için kullanılır.
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
		var request = (RequestModel)payload;

		await _dataAccessLayer.SavePushToken(
			request.UserId,
			request.PushToken,
			request.Platform.ToString(),
			request.DeviceId,
			request.AppName
		);

		return ArfBlocksResults.Success(new ResponseModel { Success = true });
	}
}
