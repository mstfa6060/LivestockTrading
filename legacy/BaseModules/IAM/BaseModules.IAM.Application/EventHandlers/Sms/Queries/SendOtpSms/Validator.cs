namespace BaseModules.IAM.Application.EventHandlers.Sms.Queries.SendOtpSms;

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
		RuleFor(x => x.PhoneNumber)
				.NotEmpty().WithMessage(DomainErrors.UserErrors.PhoneNumberRequired)
				.Matches(@"^\+?[0-9]{10,15}$").WithMessage(DomainErrors.UserErrors.PhoneNumberInvalid);


		RuleFor(x => x.Message)
			.NotEmpty().WithMessage(DomainErrors.UserErrors.MessageRequired);
	}
}
