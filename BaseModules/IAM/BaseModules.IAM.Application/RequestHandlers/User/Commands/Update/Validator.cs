namespace BaseModules.IAM.Application.RequestHandlers.Users.Commands.Update;

public class Validator : IRequestValidator
{
	private readonly IamDbValidationService _dbValidator;

	public Validator(ArfBlocksDependencyProvider dependencyProvider)
	{
		_dbValidator = dependencyProvider.GetInstance<IamDbValidationService>();
	}

	public void ValidateRequestModel(IRequestModel payload, EndpointContext context, CancellationToken cancellationToken)
	{
		var requestModel = (RequestModel)payload;

		var validationResult = new RequestModel_Validator().Validate(requestModel);
		if (!validationResult.IsValid)
		{
			var errors = validationResult.ToString("~");
			throw new ArfBlocksValidationException(errors);
		}
	}

	public async Task ValidateDomain(IRequestModel payload, EndpointContext context, CancellationToken cancellationToken)
	{
		var requestModel = (RequestModel)payload;

		// Kullanıcının var olduğunu doğrula
		await _dbValidator.ValidateUserByIdExist(requestModel.Id);
	}
}

public class RequestModel_Validator : AbstractValidator<RequestModel>
{
	public RequestModel_Validator()
	{
		RuleFor(x => x.Id)
			.NotEmpty().WithMessage(ErrorCodeGenerator.GetErrorCode(() => DomainErrors.UserErrors.UserIdIsRequired));

		RuleFor(x => x.FirstName)
			.NotEmpty().WithMessage(ErrorCodeGenerator.GetErrorCode(() => DomainErrors.UserErrors.UserFirstNameRequired))
			.MaximumLength(100)
			.When(x => x.FirstName != null);

		RuleFor(x => x.Surname)
			.NotEmpty().WithMessage(ErrorCodeGenerator.GetErrorCode(() => DomainErrors.UserErrors.UserSurnameRequired))
			.MaximumLength(100)
			.When(x => x.Surname != null);

		RuleFor(x => x.Language)
			.NotEmpty().WithMessage(ErrorCodeGenerator.GetErrorCode(() => DomainErrors.UserErrors.LanguageRequired))
			.When(x => x.Language != null);

		RuleFor(x => x.PreferredCurrencyCode)
			.NotEmpty()
			.When(x => x.PreferredCurrencyCode != null);

		RuleFor(x => x.PhoneNumber)
			.Matches(@"^\+\d{7,15}$")
			.WithMessage(ErrorCodeGenerator.GetErrorCode(() => DomainErrors.UserErrors.PhoneNumberInvalid))
			.When(x => !string.IsNullOrEmpty(x.PhoneNumber));
	}
}
