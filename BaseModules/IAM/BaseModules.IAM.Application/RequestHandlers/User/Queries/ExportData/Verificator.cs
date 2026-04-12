namespace BaseModules.IAM.Application.RequestHandlers.Users.Queries.ExportData;

public class Verificator : IRequestVerificator
{
    public Verificator(ArfBlocksDependencyProvider dependencyProvider) { }

    public async Task VerificateActor(IRequestModel payload, EndpointContext context, CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
    }

    public async Task VerificateDomain(IRequestModel payload, EndpointContext context, CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
    }
}
