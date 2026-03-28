namespace BaseModules.IAM.Application.RequestHandlers.GeoIp.Queries.DetectCountry;

public class Validator : IRequestValidator
{
    public Validator(ArfBlocksDependencyProvider dependencyProvider) { }

    public void ValidateRequestModel(IRequestModel payload, EndpointContext context, CancellationToken cancellationToken)
    {
        // No input to validate — IP comes from HTTP context
    }

    public async Task ValidateDomain(IRequestModel payload, EndpointContext context, CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
    }
}
