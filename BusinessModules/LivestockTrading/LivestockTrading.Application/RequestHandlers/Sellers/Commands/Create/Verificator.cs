using LivestockTrading.Application.Authorization;
using LivestockTrading.Infrastructure.Services;

namespace LivestockTrading.Application.RequestHandlers.Sellers.Commands.Create;

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
		// Satici basvurusu: Giris yapmis herkes basvurabilir
		// JWT authentication framework tarafindan kontrol ediliyor
		await Task.CompletedTask;
	}

	public async Task VerificateDomain(IRequestModel payload, EndpointContext context, CancellationToken cancellationToken)
	{
		await Task.CompletedTask;
	}
}
