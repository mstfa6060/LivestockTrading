using LivestockTrading.Application.Authorization;
using LivestockTrading.Infrastructure.Services;

namespace LivestockTrading.Application.RequestHandlers.SearchHistories.Commands.Delete;

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
		// All authenticated users can manage their search history
	}

	public async Task VerificateDomain(IRequestModel payload, EndpointContext context, CancellationToken cancellationToken)
	{
		var request = (RequestModel)payload;
		await _dbVerification.ValidateSearchHistoryExists(request.Id, cancellationToken);
	}
}
