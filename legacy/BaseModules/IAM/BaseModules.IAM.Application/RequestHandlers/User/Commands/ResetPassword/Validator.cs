namespace BaseModules.IAM.Application.RequestHandlers.Users.Commands.ResetPassword;

public class Validator : IRequestValidator
{
	public Validator(ArfBlocksDependencyProvider dependencyProvider) { }

	public void ValidateRequestModel(IRequestModel payload, EndpointContext context, CancellationToken cancellationToken)
	{
		var request = (RequestModel)payload;
		var result = new RequestModel_Validator().Validate(request);
		if (!result.IsValid)
			throw new ArfBlocksValidationException(result.ToString("~"));
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
		RuleFor(x => x.Token)
			.NotEmpty().WithMessage(DomainErrors.UserErrors.InvalidPassword);

		RuleFor(x => x.NewPassword)
			.MinimumLength(6).WithMessage(DomainErrors.UserErrors.PasswordTooShort);
	}
}
