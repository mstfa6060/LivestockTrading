

namespace BaseModules.IAM.Application.RequestHandlers.Users.Commands.Create;
public class Validator : IRequestValidator
{
	private readonly IamDbValidationService _dbValidator;
	public Validator(ArfBlocksDependencyProvider dependencyProvider)
	{
		_dbValidator = dependencyProvider.GetInstance<IamDbValidationService>();
	}

	public void ValidateRequestModel(IRequestModel payload, EndpointContext context, CancellationToken cancellationToken)
	{
		// Get Request Payload
		var requestModel = (RequestModel)payload;

		// Request Model Validation
		var validationResult = new RequestModel_Validator().Validate(requestModel);
		if (!validationResult.IsValid)
		{
			var errors = validationResult.ToString("~");
			throw new ArfBlocksValidationException(errors);
		}
	}
	public async Task ValidateDomain(IRequestModel payload, EndpointContext context, CancellationToken cancellationToken)
	{
		// Get Request Payload
		var requestModel = (RequestModel)payload;

		//Document
		await _dbValidator.ValidateUserByEmailExist(requestModel.Email);
		await _dbValidator.ValidateUserByUserNameExist(requestModel.UserName);

	}

}


public class RequestModel_Validator : AbstractValidator<RequestModel>
{
	public RequestModel_Validator()
	{
		RuleFor(x => x.UserName)
			.NotEmpty().WithMessage(ErrorCodeGenerator.GetErrorCode(() => DomainErrors.UserErrors.UserNameValid))
			.When(x => x.UserSource == UserSources.Manual); // Sadece manuel kayıtlar için zorunlu

		RuleFor(x => x.Email)
			.NotEmpty().WithMessage(ErrorCodeGenerator.GetErrorCode(() => DomainErrors.UserErrors.EmailValid))
			.EmailAddress().WithMessage(ErrorCodeGenerator.GetErrorCode(() => DomainErrors.UserErrors.EmailValid));

		RuleFor(x => x.Password)
			.NotEmpty().WithMessage(ErrorCodeGenerator.GetErrorCode(() => DomainErrors.UserErrors.PasswordRequired))
			.MinimumLength(6).WithMessage(ErrorCodeGenerator.GetErrorCode(() => DomainErrors.UserErrors.PasswordTooShort))
			.Matches(@"[A-Z]").WithMessage(ErrorCodeGenerator.GetErrorCode(() => DomainErrors.UserErrors.PasswordMustContainUppercase))
			.Matches(@"[a-z]").WithMessage(ErrorCodeGenerator.GetErrorCode(() => DomainErrors.UserErrors.PasswordMustContainLowercase))
			.Matches(@"\d").WithMessage(ErrorCodeGenerator.GetErrorCode(() => DomainErrors.UserErrors.PasswordMustContainDigit))
			.When(x => x.UserSource == UserSources.Manual); // Sadece manuel kayıtlar için
	}
}
