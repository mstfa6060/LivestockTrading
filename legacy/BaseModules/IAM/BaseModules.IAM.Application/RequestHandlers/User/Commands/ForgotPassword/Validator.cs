namespace BaseModules.IAM.Application.RequestHandlers.Users.Commands.ForgotPassword;

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
		var validation = new RequestModel_Validator().Validate(request);
		if (!validation.IsValid)
			throw new ArfBlocksValidationException(validation.ToString("~"));
	}

	public async Task ValidateDomain(IRequestModel payload, EndpointContext context, CancellationToken cancellationToken)
	{
		var request = (RequestModel)payload;

		// Kullanıcı varsa sessizce geç, yoksa yine de hata verme (gizlilik için)
		await _dbValidator.ValidateUserByEmailNotFound(request.Email);
		// Not: hata fırlatma — sessizce geç
		await Task.CompletedTask;
	}
}
public class RequestModel_Validator : AbstractValidator<RequestModel>
{
	public RequestModel_Validator()
	{
		RuleFor(x => x.Email)
			.NotEmpty().WithMessage(DomainErrors.UserErrors.EmailValid)
			.EmailAddress().WithMessage(DomainErrors.UserErrors.EmailValid);
	}
}
