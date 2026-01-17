namespace BaseModules.IAM.Application.RequestHandlers.Provinces.Queries.All;

public class Validator : IRequestValidator
{
    public Validator(ArfBlocksDependencyProvider dependencyProvider) { }

    public void ValidateRequestModel(IRequestModel payload, EndpointContext context, CancellationToken cancellationToken)
    {
        var requestModel = (RequestModel)payload;
        var validationResult = new RequestModel_Validator().Validate(requestModel);

        if (!validationResult.IsValid)
            throw new ArfBlocksValidationException(validationResult.ToString("~"));
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
        // Keyword opsiyonel, validasyon gerekmiyor
    }
}
