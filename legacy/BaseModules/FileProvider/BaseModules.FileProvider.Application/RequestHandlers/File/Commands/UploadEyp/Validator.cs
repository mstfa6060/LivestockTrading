namespace BaseModules.FileProvider.Application.RequestHandlers.Files.Commands.UploadEyp;

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

		if (requestModel.FormFile.ContentType != "application/octet-stream" && requestModel.FormFile.ContentType != "application/pdf")
			System.Console.WriteLine("FILE_CONTENT_TYPE_MUST_BE_EYAZISMA_OR_PDF");
		// throw new ArfBlocksValidationException("FILE_CONTENT_TYPE_MUST_BE_EYAZISMA_OR_PDF");
	}

	public async Task ValidateDomain(IRequestModel payload, EndpointContext context, CancellationToken cancellationToken)
	{
		// NOP:
		await Task.CompletedTask;
	}
}

public class RequestModel_Validator : AbstractValidator<RequestModel>
{
	public RequestModel_Validator()
	{
		RuleFor(x => x.FormFile)
			.NotNull().WithMessage(ErrorCodeGenerator.GetErrorCode(() => DomainErrors.CommonErrors.FilePayloadNotFound));
	}
}