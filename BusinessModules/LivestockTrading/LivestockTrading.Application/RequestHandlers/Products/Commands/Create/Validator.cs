using FluentValidation;
using LivestockTrading.Domain.Errors;
using LivestockTrading.Infrastructure.Services;
using Common.Services.ErrorCodeGenerator;

namespace LivestockTrading.Application.RequestHandlers.Products.Commands.Create;

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
		await _dbValidator.ValidateProductSlugUnique(request.Slug, null, cancellationToken);
		await _dbValidator.ValidateCategoryExist(request.CategoryId, cancellationToken);
		await _dbValidator.ValidateSellerExists(request.SellerId, cancellationToken);
	}
}

public class RequestModel_Validator : AbstractValidator<RequestModel>
{
	public RequestModel_Validator()
	{
		RuleFor(x => x.Title)
			.NotEmpty()
			.WithMessage(ErrorCodeGenerator.GetErrorCode(() => LivestockTradingDomainErrors.ProductErrors.TitleRequired));

		RuleFor(x => x.Slug)
			.NotEmpty()
			.WithMessage(ErrorCodeGenerator.GetErrorCode(() => LivestockTradingDomainErrors.ProductErrors.SlugRequired));

		RuleFor(x => x.CategoryId)
			.NotEmpty()
			.WithMessage(ErrorCodeGenerator.GetErrorCode(() => LivestockTradingDomainErrors.ProductErrors.CategoryRequired));

		RuleFor(x => x.SellerId)
			.NotEmpty()
			.WithMessage(ErrorCodeGenerator.GetErrorCode(() => LivestockTradingDomainErrors.ProductErrors.SellerRequired));

		RuleFor(x => x.LocationId)
			.NotEmpty()
			.WithMessage(ErrorCodeGenerator.GetErrorCode(() => LivestockTradingDomainErrors.ProductErrors.LocationRequired));

		RuleFor(x => x.BasePrice)
			.GreaterThan(0)
			.WithMessage(ErrorCodeGenerator.GetErrorCode(() => LivestockTradingDomainErrors.ProductErrors.PriceRequired));
	}
}
