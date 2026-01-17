

namespace BaseModules.Notification.Application.RequestHandlers.Push.Commands.RegisterToken;

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

		RuleFor(x => x.PushToken)
			.NotEmpty()
			.WithMessage(ErrorCodeGenerator.GetErrorCode(() => DomainErrors.UserErrors.TokenRequired));

		RuleFor(x => x.DeviceId)
			.NotEmpty()
			.WithMessage(ErrorCodeGenerator.GetErrorCode(() => DomainErrors.AuthErrors.DeviceIdNotValid));

		RuleFor(x => x.AppName)
			.NotEmpty()
			.WithMessage(ErrorCodeGenerator.GetErrorCode(() => DomainErrors.UserErrors.ProviderRequired));

		RuleFor(x => x.Platform)
			.IsInEnum()
			.WithMessage(ErrorCodeGenerator.GetErrorCode(() => DomainErrors.UserErrors.ProviderInvalid));
	}
}
