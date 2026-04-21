namespace BaseModules.IAM.Application.RequestHandlers.Roles.Commands.Delete;

public class Verificator : IRequestVerificator
{
    private readonly IamDbValidationService _dbVerificator;

    public Verificator(ArfBlocksDependencyProvider dependencyProvider)
    {
        _dbVerificator = dependencyProvider.GetInstance<IamDbValidationService>();
    }

    public async Task VerificateActor(IRequestModel payload, EndpointContext context, CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
    }

    public async Task VerificateDomain(IRequestModel payload, EndpointContext context, CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
    }
}
