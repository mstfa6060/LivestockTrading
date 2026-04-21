namespace BaseModules.IAM.Application.RequestHandlers.Auth.Commands.Logout;

/// <summary>
/// Kullanıcı Çıkış İşlemi
/// Bu endpoint, kullanıcının sistemden güvenli bir şekilde çıkış yapmasını sağlar.
/// Aktif refresh token'ı geçersiz hale getirir.
/// </summary>
public class Handler : IRequestHandler
{
	private readonly DataAccess _dataAccessLayer;
	private readonly CurrentUserService _currentUserService;

	public Handler(ArfBlocksDependencyProvider dependencyProvider, object dataAccess)
	{
		_dataAccessLayer = (DataAccess)dataAccess;
		_currentUserService = dependencyProvider.GetInstance<CurrentUserService>();
	}

	public async Task<ArfBlocksRequestResult> Handle(IRequestModel payload, EndpointContext context, CancellationToken cancellationToken)
	{
		var refreshTokenId = _currentUserService.GetCurrentRefreshTokenId();

		await _dataAccessLayer.DeleteRefreshTokenById(refreshTokenId);

		var _mapper = new Mapper();

		var response = _mapper.MapToResponse();
		return ArfBlocksResults.Success(response);
	}
}
