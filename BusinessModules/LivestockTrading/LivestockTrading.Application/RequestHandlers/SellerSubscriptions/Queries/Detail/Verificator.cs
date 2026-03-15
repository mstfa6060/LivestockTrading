using LivestockTrading.Application.Authorization;

namespace LivestockTrading.Application.RequestHandlers.SellerSubscriptions.Queries.Detail;

public class Verificator : IRequestVerificator
{
	private readonly PermissionService _permissionService;

	public Verificator(ArfBlocksDependencyProvider dependencyProvider)
	{
		_permissionService = dependencyProvider.GetInstance<PermissionService>();
	}

	public async Task VerificateActor(IRequestModel payload, EndpointContext context, CancellationToken cancellationToken)
	{
		_permissionService.RequireSeller();
	}

	public async Task VerificateDomain(IRequestModel payload, EndpointContext context, CancellationToken cancellationToken)
	{
		await Task.CompletedTask;
	}
}
