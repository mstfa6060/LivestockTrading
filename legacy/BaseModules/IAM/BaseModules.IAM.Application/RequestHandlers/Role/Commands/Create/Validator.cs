

namespace BaseModules.IAM.Application.RequestHandlers.Roles.Commands.Create;
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

		//Document
		await _dbValidator.ValidateRoleExist(requestModel.Name);

	}

}
public class RequestModel_Validator : AbstractValidator<RequestModel>
{
	public RequestModel_Validator()
	{

		RuleFor(x => x.Name)
					.NotNull().WithMessage(ErrorCodeGenerator.GetErrorCode(() => DomainErrors.RoleErrors.NameValid))
					.NotEmpty().WithMessage(ErrorCodeGenerator.GetErrorCode(() => DomainErrors.RoleErrors.NameValid));



	}
}