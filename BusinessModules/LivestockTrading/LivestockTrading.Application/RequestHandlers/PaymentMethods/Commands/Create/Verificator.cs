using LivestockTrading.Application.Authorization;
using LivestockTrading.Infrastructure.Services;

namespace LivestockTrading.Application.RequestHandlers.PaymentMethods.Commands.Create;

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

		// Rol kontrolü: Sadece Admin veya Moderator
		_permissionService.RequireModerator();
	}

	public async Task VerificateDomain(IRequestModel payload, EndpointContext context, CancellationToken cancellationToken)
	{
		await Task.CompletedTask;
	}
}
