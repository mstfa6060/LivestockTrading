using LivestockTrading.Application.Authorization;
using LivestockTrading.Infrastructure.Services;
using Common.Services.Auth.CurrentUser;

namespace LivestockTrading.Application.RequestHandlers.Transporters.Commands.Delete;

public class Verificator : IRequestVerificator
{
	private readonly PermissionService _permissionService;
	private readonly LivestockTradingModuleDbVerificationService _dbVerification;
	private readonly CurrentUserService _currentUserService;

	public Verificator(ArfBlocksDependencyProvider dependencyProvider)
	{
		_permissionService = dependencyProvider.GetInstance<PermissionService>();
		_dbVerification = dependencyProvider.GetInstance<LivestockTradingModuleDbVerificationService>();
		_currentUserService = dependencyProvider.GetInstance<CurrentUserService>();
	}

	public async Task VerificateActor(IRequestModel payload, EndpointContext context, CancellationToken cancellationToken)
	{
		// Rol kontrolü: Transporter, Admin veya Moderator
		_permissionService.RequireAnyRole(
			Constants.LivestockTradingConstants.Roles.Transporter,
			Constants.LivestockTradingConstants.Roles.Admin,
			Constants.LivestockTradingConstants.Roles.Moderator);
		await Task.CompletedTask;
	}

	public async Task VerificateDomain(IRequestModel payload, EndpointContext context, CancellationToken cancellationToken)
	{
		var request = (RequestModel)payload;
		await _dbVerification.ValidateTransporterExists(request.Id, cancellationToken);

		// Admin/Moderator can delete any transporter; Transporters can only delete their own profile
		if (!_permissionService.IsModerator())
		{
			var currentUserId = _currentUserService.GetCurrentUserId();
			await _dbVerification.ValidateTransporterOwnership(request.Id, currentUserId, cancellationToken);
		}
	}
}
