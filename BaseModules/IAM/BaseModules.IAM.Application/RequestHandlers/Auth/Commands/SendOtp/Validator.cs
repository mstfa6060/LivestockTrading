

namespace BaseModules.IAM.Application.RequestHandlers.Auth.Commands.SendOtp;

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

		await _dbValidator.GetUserById(requestModel.UserId);

		// Phone Number
		if (string.IsNullOrEmpty(requestModel.PhoneNumber))
			throw new ArfBlocksValidationException(ErrorCodeGenerator.GetErrorCode(() => DomainErrors.UserErrors.PhoneNumberRequired));

		if (!System.Text.RegularExpressions.Regex.IsMatch(requestModel.PhoneNumber, @"^\+?[0-9]{10,15}$"))
			throw new ArfBlocksValidationException(ErrorCodeGenerator.GetErrorCode(() => DomainErrors.UserErrors.PhoneNumberInvalid));

		// Language
		if (string.IsNullOrEmpty(requestModel.Language))
			throw new ArfBlocksValidationException(ErrorCodeGenerator.GetErrorCode(() => DomainErrors.UserErrors.LanguageRequired));
	}

}


public class RequestModel_Validator : AbstractValidator<RequestModel>
{
	public RequestModel_Validator()
	{
		RuleFor(x => x.PhoneNumber)
			.NotEmpty()
			.WithMessage(ErrorCodeGenerator.GetErrorCode(() => DomainErrors.UserErrors.PhoneNumberRequired))
			.Matches(@"^\+?[0-9]{10,15}$")
			.WithMessage(ErrorCodeGenerator.GetErrorCode(() => DomainErrors.UserErrors.PhoneNumberInvalid));

		RuleFor(x => x.Language)
			.NotEmpty()
			.WithMessage(ErrorCodeGenerator.GetErrorCode(() => DomainErrors.UserErrors.LanguageRequired));
	}
}
