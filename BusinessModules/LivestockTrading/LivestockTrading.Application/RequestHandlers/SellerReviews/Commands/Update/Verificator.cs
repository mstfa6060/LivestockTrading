using LivestockTrading.Application.Authorization;
using LivestockTrading.Application.Constants;
using LivestockTrading.Infrastructure.Services;

namespace LivestockTrading.Application.RequestHandlers.SellerReviews.Commands.Update;

public class Verificator : IRequestVerificator
{
	private readonly AuthorizationService _authorizationService;
	private readonly LivestockTradingModuleDbVerificationService _dbVerification;
	private readonly PermissionService _permissionService;

	public Verificator(ArfBlocksDependencyProvider dependencyProvider)
	{
		_authorizationService = dependencyProvider.GetInstance<AuthorizationService>();
		_dbVerification = dependencyProvider.GetInstance<LivestockTradingModuleDbVerificationService>();
		_permissionService = dependencyProvider.GetInstance<PermissionService>();
	}

	public async Task VerificateActor(IRequestModel payload, EndpointContext context, CancellationToken cancellationToken)
	{
		await _authorizationService
			.ForResource(typeof(Verificator).Namespace)
			.VerifyActor()
			.Assert();

		_permissionService.RequireAnyRole(
			LivestockTradingConstants.Roles.Buyer,
			LivestockTradingConstants.Roles.Admin,
			LivestockTradingConstants.Roles.Moderator);
	}

	public async Task VerificateDomain(IRequestModel payload, EndpointContext context, CancellationToken cancellationToken)
	{
		var request = (RequestModel)payload;
		await _dbVerification.ValidateSellerReviewExists(request.Id, cancellationToken);
	}
}
