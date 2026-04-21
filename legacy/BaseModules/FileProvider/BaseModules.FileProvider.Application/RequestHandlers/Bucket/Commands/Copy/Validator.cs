namespace BaseModules.FileProvider.Application.RequestHandlers.Buckets.Commands.Copy;

public class Validator : IRequestValidator
{
	public Validator(ArfBlocksDependencyProvider dependencyProvider)
	{ }

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
		//NOP:
		await Task.CompletedTask;
	}
}

public class RequestModel_Validator : AbstractValidator<RequestModel>
{
	public RequestModel_Validator()
	{
		RuleFor(x => x.BucketId)
			.NotNull().WithMessage(ErrorCodeGenerator.GetErrorCode(() => DomainErrors.BucketErrors.BucketIdIsNull))
			.NotEmpty().WithMessage(ErrorCodeGenerator.GetErrorCode(() => DomainErrors.BucketErrors.BuckedIdIsEmptyString));

		RuleFor(x => x.ChangeId)
			.NotNull().WithMessage(ErrorCodeGenerator.GetErrorCode(() => DomainErrors.BucketErrors.ChangeIdIsNull));
	}
}