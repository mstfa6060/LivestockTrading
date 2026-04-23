namespace BaseModules.IAM.Application.RequestHandlers.Auth.Commands.Logout;

public class Validator : IRequestValidator
{
	private readonly IamDbValidationService _dbValidator;
	private readonly CurrentUserService _currentUserService;

	public Validator(ArfBlocksDependencyProvider dependencyProvider)
	{
		_dbValidator = dependencyProvider.GetInstance<IamDbValidationService>();
		_currentUserService = dependencyProvider.GetInstance<CurrentUserService>();
	}



	public void ValidateRequestModel(IRequestModel payload, EndpointContext context, CancellationToken cancellationToken)
	{
		var requestModel = (RequestModel)payload;

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

		var userId = _currentUserService.GetCurrentUserId(); // Eğer burada erişiyorsan
		var refreshTokenId = _currentUserService.GetCurrentRefreshTokenId();

		await _dbValidator.GetUserById(userId);

		await _dbValidator.ValidateRefreshTokenExist(refreshTokenId); //  En doğru kullanım

	}


	public class RequestModel_Validator : AbstractValidator<RequestModel>
	{
		public RequestModel_Validator()
		{

		}
	}

}
