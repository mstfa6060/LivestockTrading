namespace BaseModules.IAM.Application.RequestHandlers.Auth.Commands.RevokeRefreshToken;

public class Validator : IRequestValidator
{
	private readonly IamDbValidationService _dbValidator;

	public Validator(ArfBlocksDependencyProvider dependencyProvider)
	{
		_dbValidator = dependencyProvider.GetInstance<IamDbValidationService>();
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
		var request = (RequestModel)payload;
		await _dbValidator.ValidateRefreshTokenExist(request.RefreshTokenId);
	}
}

public class RequestModel_Validator : AbstractValidator<RequestModel>
{
	public RequestModel_Validator()
	{
		RuleFor(x => x.RefreshTokenId)
			.NotEmpty().WithMessage(ErrorCodeGenerator.GetErrorCode(() => DomainErrors.UserErrors.RefreshTokenRequired));
	}
}
