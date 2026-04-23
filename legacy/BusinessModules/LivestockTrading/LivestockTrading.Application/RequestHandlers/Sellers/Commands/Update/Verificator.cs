using LivestockTrading.Application.Authorization;
using LivestockTrading.Infrastructure.Services;
using Common.Services.Auth.CurrentUser;

namespace LivestockTrading.Application.RequestHandlers.Sellers.Commands.Update;

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
		// Rol kontrolü: Seller, Admin veya Moderator
		_permissionService.RequireAnyRole(
			Constants.LivestockTradingConstants.Roles.Seller,
			Constants.LivestockTradingConstants.Roles.Admin,
			Constants.LivestockTradingConstants.Roles.Moderator);
		await Task.CompletedTask;
	}

	public async Task VerificateDomain(IRequestModel payload, EndpointContext context, CancellationToken cancellationToken)
	{
		var request = (RequestModel)payload;
		await _dbVerification.ValidateSellerExists(request.Id, cancellationToken);

		// Admin/Moderator can update any seller; Sellers can only update their own profile
		if (!_permissionService.IsModerator())
		{
			var currentUserId = _currentUserService.GetCurrentUserId();
			await _dbVerification.ValidateSellerOwnership(request.Id, currentUserId, cancellationToken);
		}
	}
}
