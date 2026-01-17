using Microsoft.Extensions.Configuration;

namespace BaseModules.IAM.Application.RequestHandlers.Auth.Commands.Login;

public class Validator : IRequestValidator
{
	private readonly IamDbValidationService _dbValidator;
	private readonly ExternalAuthConfiguration _configuration;

	public Validator(ArfBlocksDependencyProvider dependencyProvider)
	{
		_dbValidator = dependencyProvider.GetInstance<IamDbValidationService>();
		_configuration = dependencyProvider.GetInstance<ExternalAuthConfiguration>();
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

		if (request.Provider?.ToLower() == "google")
		{
			// var googleUserId = await _dbValidator.ValidateGoogleToken(request.Token, _configuration.Google.ClientId);
			// if (string.IsNullOrEmpty(googleUserId))
			// 	throw new ArfBlocksValidationException(ErrorCodeGenerator.GetErrorCode(() => DomainErrors.UserErrors.GoogleTokenInvalid));
			// request.ExternalProviderUserId = googleUserId; // SUB -> Model'e yaz
		}
		else if (request.Provider?.ToLower() == "apple" || request.Provider?.ToLower() == "itunes")
		{
			// var appleUserId = await _dbValidator.ValidateAppleToken(request.Token, _configuration.Apple.ClientId, _configuration.Apple.ClientSecret);
			// if (string.IsNullOrEmpty(appleUserId))
			// 	throw new ArfBlocksValidationException(ErrorCodeGenerator.GetErrorCode(() => DomainErrors.UserErrors.AppleTokenInvalid));
			// request.ExternalProviderUserId = appleUserId; // SUB -> Model'e yaz
		}

		await Task.CompletedTask;
	}

}

public class RequestModel_Validator : AbstractValidator<RequestModel>
{
	public RequestModel_Validator()
	{
		RuleFor(x => x.Provider)
			.NotEmpty().WithMessage(ErrorCodeGenerator.GetErrorCode(() => DomainErrors.UserErrors.ProviderRequired));

		When(x => x.Provider.ToLower() == "native", () =>
		{
			RuleFor(x => x.UserName).NotEmpty().WithMessage(ErrorCodeGenerator.GetErrorCode(() => DomainErrors.UserErrors.UserNameRequired));
			RuleFor(x => x.Password).NotEmpty().WithMessage(ErrorCodeGenerator.GetErrorCode(() => DomainErrors.UserErrors.PasswordRequired));
		});

		When(x => x.Provider.ToLower() == "google" || x.Provider.ToLower() == "itunes", () =>
		{
			RuleFor(x => x.Token).NotEmpty().WithMessage(ErrorCodeGenerator.GetErrorCode(() => DomainErrors.UserErrors.TokenRequired));
		});
	}
}
