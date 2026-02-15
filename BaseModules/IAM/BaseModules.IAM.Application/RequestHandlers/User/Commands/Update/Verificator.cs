using Common.Services.Auth.CurrentUser;

namespace BaseModules.IAM.Application.RequestHandlers.Users.Commands.Update;

public class Verificator : IRequestVerificator
{
	private readonly AuthorizationService _authorizationService;
	private readonly IamDbVerificationService _dbVerification;
	private readonly CurrentUserService _currentUserService;

	public Verificator(ArfBlocksDependencyProvider dependencyProvider)
	{
		_authorizationService = dependencyProvider.GetInstance<AuthorizationService>();
		_dbVerification = dependencyProvider.GetInstance<IamDbVerificationService>();
		_currentUserService = dependencyProvider.GetInstance<CurrentUserService>();
	}

	public async Task VerificateActor(IRequestModel payload, EndpointContext context, CancellationToken cancellationToken)
	{
		await _authorizationService
			.ForResource(typeof(Verificator).Namespace)
			.VerifyActor()
			.Assert();

		// JWT'deki userId == request.Id olmalı VEYA kullanıcı Admin rolünde olmalı
		var request = (RequestModel)payload;
		var currentUserId = _currentUserService.GetCurrentUserId();
		var isAdmin = _currentUserService.HasRoleInModule("LivestockTrading", "Admin");

		if (currentUserId != request.Id && !isAdmin)
			throw new ArfBlocksValidationException(
				ErrorCodeGenerator.GetErrorCode(() => DomainErrors.AuthErrors.UserCanOnlyMakeActionsForSelf));
	}

	public async Task VerificateDomain(IRequestModel payload, EndpointContext context, CancellationToken cancellationToken)
	{
		var request = (RequestModel)payload;

		// Kullanıcının varlığını doğrula
		await _dbVerification.VerifyUserCanLogin(request.Id);
	}
}
