namespace BaseModules.IAM.Application.RequestHandlers.Companies.Queries.Pick;

public class Validator : IRequestValidator
{
	public Validator(ArfBlocksDependencyProvider _) { }

	public void ValidateRequestModel(IRequestModel payload, EndpointContext context, CancellationToken cancellationToken)
	{
		var requestModel = (RequestModel)payload;

		var result = new RequestModel_Validator().Validate(requestModel);
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
		RuleFor(x => x.Limit)
			.GreaterThan(0).When(x => x.Limit != 0)
			.WithMessage(ErrorCodeGenerator.GetErrorCode(() => DomainErrors.CompanyErrors.PickLimitInvalid));
	}
}
