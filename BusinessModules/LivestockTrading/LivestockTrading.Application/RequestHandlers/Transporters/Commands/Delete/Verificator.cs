using LivestockTrading.Application.Authorization;
using LivestockTrading.Infrastructure.Services;

namespace LivestockTrading.Application.RequestHandlers.Transporters.Commands.Delete;

public class Verificator : IRequestVerificator
{
	private readonly PermissionService _permissionService;
	private readonly LivestockTradingModuleDbVerificationService _dbVerification;

	public Verificator(ArfBlocksDependencyProvider dependencyProvider)
	{
		_permissionService = dependencyProvider.GetInstance<PermissionService>();
		_dbVerification = dependencyProvider.GetInstance<LivestockTradingModuleDbVerificationService>();
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
		// TODO: Transporter ise kendi profili mi kontrolü eklenecek
	}
}
