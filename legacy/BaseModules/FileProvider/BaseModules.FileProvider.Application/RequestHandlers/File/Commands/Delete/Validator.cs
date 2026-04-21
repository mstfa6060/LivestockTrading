namespace BaseModules.FileProvider.Application.RequestHandlers.Files.Commands.Delete;

public class Validator : IRequestValidator
{
	private readonly FileProviderDocumentDbValidationService _documentDbValidationService;
	public Validator(ArfBlocksDependencyProvider dependencyProvider)
	{
		_documentDbValidationService = dependencyProvider.GetInstance<FileProviderDocumentDbValidationService>();
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
		var requestPayload = (RequestModel)payload;

		// Validate File
		await _documentDbValidationService.ValidateBucketExist(requestPayload.BucketId);
		await _documentDbValidationService.ValidateFileEntryExist(requestPayload.BucketId, requestPayload.FileId);
	}
}

public class RequestModel_Validator : AbstractValidator<RequestModel>
{
	public RequestModel_Validator()
	{
		RuleFor(x => x.FileId)
			.NotNull().WithMessage(ErrorCodeGenerator.GetErrorCode(() => DomainErrors.CommonErrors.FileEntryIdNotValid))
			.NotEqual(Guid.Empty).WithMessage(ErrorCodeGenerator.GetErrorCode(() => DomainErrors.CommonErrors.FileEntryIdNotValid));

		RuleFor(x => x.BucketId)
			.NotNull().WithMessage(ErrorCodeGenerator.GetErrorCode(() => DomainErrors.CommonErrors.BucketIdNotValid))
			.NotEmpty().WithMessage(ErrorCodeGenerator.GetErrorCode(() => DomainErrors.CommonErrors.BucketIdNotValid));
	}
}