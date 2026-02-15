namespace BaseModules.IAM.Application.RequestHandlers.Countries.Queries.GetByCode;

public class Validator : IRequestValidator
{
    public Validator(ArfBlocksDependencyProvider dependencyProvider) { }

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
        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("Country code is required.")
            .MaximumLength(2).WithMessage("Country code must be 2 characters (ISO 3166-1 alpha-2).");
    }
}
