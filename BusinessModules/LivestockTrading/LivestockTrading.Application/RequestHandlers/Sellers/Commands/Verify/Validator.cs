using FluentValidation;
using LivestockTrading.Domain.Entities;
using LivestockTrading.Domain.Errors;
using LivestockTrading.Infrastructure.Services;
using Common.Services.ErrorCodeGenerator;

namespace LivestockTrading.Application.RequestHandlers.Sellers.Commands.Verify;

public class Validator : IRequestValidator
{
	private readonly LivestockTradingModuleDbValidationService _dbValidator;

	public Validator(ArfBlocksDependencyProvider dependencyProvider)
	{
		_dbValidator = dependencyProvider.GetInstance<LivestockTradingModuleDbValidationService>();
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

		// Check seller exists
		await _dbValidator.ValidateSellerExists(request.SellerId, cancellationToken);

		// Check seller is in pending status
		await _dbValidator.ValidateSellerIsPendingVerification(request.SellerId, cancellationToken);
	}
}

public class RequestModel_Validator : AbstractValidator<RequestModel>
{
	public RequestModel_Validator()
	{
		RuleFor(x => x.SellerId)
			.NotEmpty()
			.WithMessage(ErrorCodeGenerator.GetErrorCode(() => LivestockTradingDomainErrors.CommonErrors.IdNotValid));
	}
}
