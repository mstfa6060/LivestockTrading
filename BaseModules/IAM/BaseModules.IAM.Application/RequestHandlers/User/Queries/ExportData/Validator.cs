namespace BaseModules.IAM.Application.RequestHandlers.Users.Queries.ExportData;

public class Validator : IRequestValidator
{
    public Validator(ArfBlocksDependencyProvider dependencyProvider) { }

    public void ValidateRequestModel(IRequestModel payload, EndpointContext context, CancellationToken cancellationToken)
    {
        var requestModel = (RequestModel)payload;
        if (requestModel.UserId == Guid.Empty)
            throw new ArfBlocksValidationException(
                ErrorCodeGenerator.GetErrorCode(() => DomainErrors.UserErrors.UserIdIsRequired));
    }

    public async Task ValidateDomain(IRequestModel payload, EndpointContext context, CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
    }
}
