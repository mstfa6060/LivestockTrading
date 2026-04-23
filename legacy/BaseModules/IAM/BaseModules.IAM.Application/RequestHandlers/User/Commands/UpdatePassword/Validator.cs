

namespace BaseModules.IAM.Application.RequestHandlers.Users.Commands.UpdatePassword;

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


		var user = await _dbValidator.GetUserById(requestModel.UserId); //  await eklendi
		if (user == null)
			throw new ArfBlocksValidationException(DomainErrors.UserErrors.UserNotFound);

		var isPasswordCorrect = SecurityHelper.VerifyPassword(requestModel.OldPassword, user.PasswordHash, user.PasswordSalt);
		if (!isPasswordCorrect)
			throw new ArfBlocksValidationException(DomainErrors.UserErrors.InvalidPassword);

		//Document
		await _dbValidator.ValidateUserByIdExist(requestModel.UserId);

	}

}

public class RequestModel_Validator : AbstractValidator<RequestModel>
{
	public RequestModel_Validator()
	{
		RuleFor(x => x.UserId)
			.NotEmpty().WithMessage(DomainErrors.UserErrors.UserIdIsRequired);

		RuleFor(x => x.OldPassword)
			.NotEmpty().WithMessage(DomainErrors.UserErrors.PasswordIsRequired);

		RuleFor(x => x.NewPassword)
			.MinimumLength(6).WithMessage(DomainErrors.UserErrors.PasswordTooShort);
	}
}
