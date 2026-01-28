using LivestockTrading.Application.Authorization;
using LivestockTrading.Infrastructure.Services;

namespace LivestockTrading.Application.RequestHandlers.FeedInfos.Commands.Delete;

public class Verificator : IRequestVerificator
{
	private readonly AuthorizationService _authorizationService;
	private readonly PermissionService _permissionService;
	private readonly LivestockTradingModuleDbVerificationService _dbVerification;

	public Verificator(ArfBlocksDependencyProvider dependencyProvider)
	{
		_authorizationService = dependencyProvider.GetInstance<AuthorizationService>();
		_permissionService = dependencyProvider.GetInstance<PermissionService>();
		_dbVerification = dependencyProvider.GetInstance<LivestockTradingModuleDbVerificationService>();
	}

	public async Task VerificateActor(IRequestModel payload, EndpointContext context, CancellationToken cancellationToken)
	{
		await _authorizationService
			.ForResource(typeof(Verificator).Namespace)
			.VerifyActor()
			.Assert();

		// Rol kontrolü: Seller, Admin veya Moderator
		_permissionService.RequireAnyRole(
			Constants.LivestockTradingConstants.Roles.Seller,
			Constants.LivestockTradingConstants.Roles.Admin,
			Constants.LivestockTradingConstants.Roles.Moderator);
	}

	public async Task VerificateDomain(IRequestModel payload, EndpointContext context, CancellationToken cancellationToken)
	{
		var request = (RequestModel)payload;
		await _dbVerification.ValidateFeedInfoExists(request.Id, cancellationToken);
	}
}
