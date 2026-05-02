using FluentValidation;
using LivestockTrading.Domain.Errors;
using LivestockTrading.Infrastructure.Services;
using Common.Services.ErrorCodeGenerator;

namespace LivestockTrading.Application.RequestHandlers.SellerSubscriptions.Commands.AdminAssign;

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
		await _dbValidator.ValidateSellerExists(request.SellerId, cancellationToken);
		await _dbValidator.ValidateSubscriptionPlanExists(request.SubscriptionPlanId, cancellationToken);
	}
}

public class RequestModel_Validator : AbstractValidator<RequestModel>
{
	public RequestModel_Validator()
	{
		RuleFor(x => x.SellerId)
			.NotEmpty()
			.WithMessage(ErrorCodeGenerator.GetErrorCode(() => LivestockTradingDomainErrors.SellerSubscriptionErrors.SellerSubscriptionSellerRequired));

		RuleFor(x => x.SubscriptionPlanId)
			.NotEmpty()
			.WithMessage(ErrorCodeGenerator.GetErrorCode(() => LivestockTradingDomainErrors.SellerSubscriptionErrors.SellerSubscriptionPlanRequired));

		RuleFor(x => x.Period)
			.InclusiveBetween(0, 1);
	}
}
