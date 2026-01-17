namespace BaseModules.IAM.Application.RequestHandlers.Auth.Commands.VerifyOtp;

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
		var request = (RequestModel)payload;

		// Şirket doğrulaması
		await _dbValidator.ValidateCompanyExist(request.CompanyId);

		// Kullanıcı + OTP doğrulama
		await _dbValidator.ValidateUserOtpVerification(
			userId: request.UserId,
			phoneNumber: request.PhoneNumber,
			companyId: request.CompanyId,
			otpCode: request.OtpCode
		);
	}
}

public class RequestModel_Validator : AbstractValidator<RequestModel>
{
	public RequestModel_Validator()
	{
		RuleFor(x => x.PhoneNumber)
			.NotEmpty().WithMessage("PhoneNumber.Required")
			.Matches(@"^\+?[0-9]{10,15}$").WithMessage("PhoneNumber.Invalid");

		RuleFor(x => x.CompanyId)
			.NotEmpty().WithMessage("CompanyId.Required");

		RuleFor(x => x.OtpCode)
			.NotEmpty().WithMessage("OtpCode.Required")
			.Length(6).WithMessage("OtpCode.LengthMustBe6");
	}
}
