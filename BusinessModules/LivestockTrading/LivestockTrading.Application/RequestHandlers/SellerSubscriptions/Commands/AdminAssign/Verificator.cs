using LivestockTrading.Application.Authorization;

namespace LivestockTrading.Application.RequestHandlers.SellerSubscriptions.Commands.AdminAssign;

public class Verificator : IRequestVerificator
{
	private readonly PermissionService _permissionService;

	public Verificator(ArfBlocksDependencyProvider dependencyProvider)
	{
		_permissionService = dependencyProvider.GetInstance<PermissionService>();
	}

	public async Task VerificateActor(IRequestModel payload, EndpointContext context, CancellationToken cancellationToken)
	{
		// Admin / Moderator ozel: odeme akisi olmadan plan atadigimiz icin yetki kapaliyiz.
		_permissionService.RequireModerator();
		await Task.CompletedTask;
	}

	public async Task VerificateDomain(IRequestModel payload, EndpointContext context, CancellationToken cancellationToken)
	{
		await Task.CompletedTask;
	}
}
