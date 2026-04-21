namespace BaseModules.FileProvider.Application.RequestHandlers.Files.Commands.Upload;

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

		//TODO: Ask where to put.
		// if (requestModel.ModuleName == "cis"
		// 		&& (requestModel.FolderName.Contains("/valuableDocuments/") || requestModel.FolderName.Contains("/licenses/"))
		// 		&& requestModel.FormFile.ContentType != ".pdf")
		// {
		// 	throw new ArfBlocksValidationException(ErrorCodeGenerator.GetErrorCode(() => DomainErrors.BucketErrors.InvalidaFileExtension));
		// }
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

		RuleFor(x => x.FormFile.Length)
				.GreaterThan(0).WithMessage(ErrorCodeGenerator.GetErrorCode(() => DomainErrors.CommonErrors.FilePayloadIsEmpty));
	}
}