namespace BaseModules.IAM.Application.RequestHandlers.Countries.Queries.GetByCode;

public class Verificator : IRequestVerificator
{
    public Verificator(ArfBlocksDependencyProvider _) { }

    public async Task VerificateActor(IRequestModel payload, EndpointContext context, CancellationToken cancellationToken)
    {
        // Public endpoint - no authentication required
        await Task.CompletedTask;
    }

    public async Task VerificateDomain(IRequestModel payload, EndpointContext context, CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
    }
}
