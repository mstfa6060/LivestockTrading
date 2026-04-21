namespace BaseModules.IAM.Application.RequestHandlers.Users.Commands.Delete;

public class Validator : IRequestValidator
{
	private readonly IamDbValidationService _dbValidator;
	private readonly CurrentUserService _currentUserService;

	public Validator(ArfBlocksDependencyProvider dependencyProvider)
	{
		_dbValidator = dependencyProvider.GetInstance<IamDbValidationService>();
		_currentUserService = dependencyProvider.GetInstance<CurrentUserService>();
	}

	public void ValidateRequestModel(IRequestModel payload, EndpointContext context, CancellationToken cancellationToken)
	{
		var request = (RequestModel)payload;
		var validationResult = new RequestModel_Validator().Validate(request);
		if (!validationResult.IsValid)
			throw new ArfBlocksValidationException(validationResult.ToString("~"));
	}

	public async Task ValidateDomain(IRequestModel payload, EndpointContext context, CancellationToken cancellationToken)
	{
		var currentUserId = _currentUserService.GetCurrentUserId();

		// Verify the user exists and is not already deleted
		var user = await _dbValidator.GetUserById(currentUserId);
		if (user.IsDeleted)
			throw new ArfBlocksValidationException(
				ErrorCodeGenerator.GetErrorCode(() => DomainErrors.UserErrors.UserDeleteAlreadyDeleted));

		// Verify password is correct
		var isPasswordCorrect = SecurityHelper.VerifyPassword(((RequestModel)payload).Password, user.PasswordHash, user.PasswordSalt);
		if (!isPasswordCorrect)
			throw new ArfBlocksValidationException(
				ErrorCodeGenerator.GetErrorCode(() => DomainErrors.UserErrors.UserDeletePasswordIncorrect));
	}
}

public class RequestModel_Validator : AbstractValidator<RequestModel>
{
	public RequestModel_Validator()
	{
		RuleFor(x => x.Password)
			.NotEmpty()
			.WithMessage(ErrorCodeGenerator.GetErrorCode(() => DomainErrors.UserErrors.UserDeletePasswordRequired));
	}
}
