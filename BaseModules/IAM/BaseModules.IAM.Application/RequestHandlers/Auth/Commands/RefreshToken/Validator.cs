using Common.Definitions.Domain.Errors;
using Common.Services.Auth;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace BaseModules.IAM.Application.RequestHandlers.Auth.Commands.RefreshToken;

public class Validator : IRequestValidator
{
	private readonly IamDbValidationService _dbValidator;


	public Validator(ArfBlocksDependencyProvider dependencyProvider)
	{
		_dbValidator = dependencyProvider.GetInstance<IamDbValidationService>();
	}

	public void ValidateRequestModel(IRequestModel payload, EndpointContext context, CancellationToken cancellationToken)
	{
		var request = (RequestModel)payload;
		var result = new RequestModel_Validator().Validate(request);
		if (!result.IsValid)
			throw new ArfBlocksValidationException(result.ToString("~"));
	}

	public async Task ValidateDomain(IRequestModel payload, EndpointContext context, CancellationToken cancellationToken)
	{
		var request = (RequestModel)payload;

		await _dbValidator.GetUserByRefreshTokenAsync(request.RefreshToken);

	}
}

public class RequestModel_Validator : AbstractValidator<RequestModel>
{
	public RequestModel_Validator()
	{
		RuleFor(x => x.RefreshToken)
			.NotEmpty()
			.WithMessage(ErrorCodeGenerator.GetErrorCode(() => DomainErrors.UserErrors.RefreshTokenRequired));
	}
}
