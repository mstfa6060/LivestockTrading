namespace BaseModules.IAM.Application.RequestHandlers.Geo.Queries.Detect;

public class Verificator : IRequestVerificator
{
    public Verificator(ArfBlocksDependencyProvider dependencyProvider)
    {
    }

    public async Task VerificateActor(IRequestModel payload, EndpointContext context, CancellationToken cancellationToken)
    {
        // Public endpoint - no auth required
        await Task.CompletedTask;
    }

    public async Task VerificateDomain(IRequestModel payload, EndpointContext context, CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
    }
}
