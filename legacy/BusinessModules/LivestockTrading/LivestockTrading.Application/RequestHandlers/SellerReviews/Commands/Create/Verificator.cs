using LivestockTrading.Application.Authorization;
using LivestockTrading.Application.Constants;
using LivestockTrading.Infrastructure.Services;

namespace LivestockTrading.Application.RequestHandlers.SellerReviews.Commands.Create;

public class Verificator : IRequestVerificator
{
	private readonly LivestockTradingModuleDbVerificationService _dbVerification;
	private readonly PermissionService _permissionService;

	public Verificator(ArfBlocksDependencyProvider dependencyProvider)
	{
		_dbVerification = dependencyProvider.GetInstance<LivestockTradingModuleDbVerificationService>();
		_permissionService = dependencyProvider.GetInstance<PermissionService>();
	}

	public async Task VerificateActor(IRequestModel payload, EndpointContext context, CancellationToken cancellationToken)
	{
		_permissionService.RequireAnyRole(
			LivestockTradingConstants.Roles.Buyer,
			LivestockTradingConstants.Roles.Admin);
	}

	public async Task VerificateDomain(IRequestModel payload, EndpointContext context, CancellationToken cancellationToken)
	{
		await Task.CompletedTask;
	}
}
