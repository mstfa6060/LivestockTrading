namespace BaseModules.IAM.Application.RequestHandlers.UserPermissions.Queries.My;

/// <summary>
/// Kullanıcının Yetkilerini Getir
/// Bu endpoint, kullanıcının sahip olduğu tüm yetkileri döner.
/// Frontend'te menü ve component rendering için kullanılır.
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

	public async Task<ArfBlocksRequestResult> Handle(
		IRequestModel payload,
		EndpointContext context,
		CancellationToken cancellationToken)
	{
		var mapper = new Mapper();
		var userId = _currentUserService.GetCurrentUserId();

		// Kullanıcının rollerini al
		var userRoleIds = await _dataAccessLayer.GetUserRoleIds(userId);

		// Rollere ait permissionları al
		var permissions = await _dataAccessLayer.GetRolePermissions(userRoleIds);

		// Resource'ları al (endpoint yetkileri)
		var resources = await _dataAccessLayer.GetUserResources(userId);

		var response = mapper.MapToResponse(permissions, resources);
		return ArfBlocksResults.Success(response);
	}
}