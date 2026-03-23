using LivestockTrading.Application.Authorization;

namespace LivestockTrading.Application.RequestHandlers.ProductReports.Commands.Update;

public class Verificator : IRequestVerificator
{
	private readonly PermissionService _permissionService;

	public Verificator(ArfBlocksDependencyProvider dependencyProvider)
	{
		_permissionService = dependencyProvider.GetInstance<PermissionService>();
	}

	public async Task VerificateActor(IRequestModel payload, EndpointContext context, CancellationToken cancellationToken)
	{
		// Only Admin or Moderator can update product reports
		_permissionService.RequireAnyRole(
			Constants.LivestockTradingConstants.Roles.Admin,
			Constants.LivestockTradingConstants.Roles.Moderator);
		await Task.CompletedTask;
	}

	public async Task VerificateDomain(IRequestModel payload, EndpointContext context, CancellationToken cancellationToken)
	{
		await Task.CompletedTask;
	}
}
