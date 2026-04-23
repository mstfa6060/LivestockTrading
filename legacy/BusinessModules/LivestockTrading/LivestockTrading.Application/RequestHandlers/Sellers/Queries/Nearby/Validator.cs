using FluentValidation;

namespace LivestockTrading.Application.RequestHandlers.Sellers.Queries.Nearby;

public class Validator : IRequestValidator
{
	public Validator(ArfBlocksDependencyProvider dependencyProvider)
	{
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
		await Task.CompletedTask;
	}
}

public class RequestModel_Validator : AbstractValidator<RequestModel>
{
	public RequestModel_Validator()
	{
		RuleFor(x => x.Latitude)
			.InclusiveBetween(-90.0, 90.0)
			.WithMessage("Latitude must be between -90 and 90.");

		RuleFor(x => x.Longitude)
			.InclusiveBetween(-180.0, 180.0)
			.WithMessage("Longitude must be between -180 and 180.");
	}
}
