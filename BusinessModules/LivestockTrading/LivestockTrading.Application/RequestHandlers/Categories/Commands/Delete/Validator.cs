using FluentValidation;

namespace LivestockTrading.Application.RequestHandlers.Categories.Commands.Delete;

public class Validator : IRequestValidator
{
	public Validator(ArfBlocksDependencyProvider dependencyProvider)
	{
	}

	public void ValidateRequestModel(IRequestModel payload, EndpointContext context, CancellationToken cancellationToken)
	{
		var request = (RequestModel)payload;
		var result = new RequestModelValidator().Validate(request);
		if (!result.IsValid)
			throw new ArfBlocksValidationException(result.ToString("~"));
	}

	public async Task ValidateDomain(IRequestModel payload, EndpointContext context, CancellationToken cancellationToken)
	{
	}
}

public class RequestModelValidator : AbstractValidator<RequestModel>
{
	public RequestModelValidator()
	{
		RuleFor(x => x.Id)
			.NotEmpty().WithMessage("CATEGORY_ID_REQUIRED");
	}
}
