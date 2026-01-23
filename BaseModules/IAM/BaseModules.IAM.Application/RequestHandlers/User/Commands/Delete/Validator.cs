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
		await Task.CompletedTask;
	}
}

public class RequestModel_Validator : AbstractValidator<RequestModel>
{
	public RequestModel_Validator()
	{
		RuleFor(x => x.UserId)
			.NotEmpty()
			.WithMessage(ErrorCodeGenerator.GetErrorCode(() => DomainErrors.AuthErrors.UserIdNotValid));
	}
}
