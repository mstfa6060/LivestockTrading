namespace BaseModules.IAM.Application.RequestHandlers.Auth.Commands.RevokeRefreshToken;

/// <summary>
/// Refresh Token İptal Etme
/// Bu endpoint, belirli bir refresh token'ı geçersiz kılar.
/// Güvenlik amaçlı kullanıcı oturumlarını sonlandırmak için kullanılır.
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

		await _dataAccessLayer.RevokeToken(request.RefreshTokenId);

		var response = new ResponseModel { Success = true };
		return ArfBlocksResults.Success(response);
	}
}
