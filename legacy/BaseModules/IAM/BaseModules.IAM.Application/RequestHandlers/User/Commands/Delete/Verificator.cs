namespace BaseModules.IAM.Application.RequestHandlers.Users.Commands.Delete;

public class Verificator : IRequestVerificator
{
	private readonly AuthorizationService _authorizationService;
	private readonly CurrentUserService _currentUserService;

	public Verificator(ArfBlocksDependencyProvider dependencyProvider)
	{
		_authorizationService = dependencyProvider.GetInstance<AuthorizationService>();
		_currentUserService = dependencyProvider.GetInstance<CurrentUserService>();
	}

	public async Task VerificateActor(IRequestModel payload, EndpointContext context, CancellationToken cancellationToken)
	{
		// Verify the user is authenticated
		var currentUserId = _currentUserService.GetCurrentUserId();
		if (currentUserId == Guid.Empty)
			throw new ArfBlocksValidationException(
				ErrorCodeGenerator.GetErrorCode(() => DomainErrors.UserErrors.UserDeleteNotAuthenticated));

		await Task.CompletedTask;
	}

	public async Task VerificateDomain(IRequestModel payload, EndpointContext context, CancellationToken cancellationToken)
	{
		await Task.CompletedTask;
	}
}
