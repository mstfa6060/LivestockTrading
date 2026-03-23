using Common.Services.Auth.CurrentUser;

namespace LivestockTrading.Application.RequestHandlers.ProductReports.Commands.Create;

public class Verificator : IRequestVerificator
{
	private readonly CurrentUserService _currentUserService;

	public Verificator(ArfBlocksDependencyProvider dependencyProvider)
	{
		_currentUserService = dependencyProvider.GetInstance<CurrentUserService>();
	}

	public async Task VerificateActor(IRequestModel payload, EndpointContext context, CancellationToken cancellationToken)
	{
		var userId = _currentUserService.GetCurrentUserId();
		if (userId == Guid.Empty)
			throw new ArfBlocksValidationException("Authentication required");

		await Task.CompletedTask;
	}

	public async Task VerificateDomain(IRequestModel payload, EndpointContext context, CancellationToken cancellationToken)
	{
		await Task.CompletedTask;
	}
}
