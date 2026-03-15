namespace LivestockTrading.Application.RequestHandlers.ProductBoosts.Commands.Expire;

public class Verificator : IRequestVerificator
{
    public Verificator(ArfBlocksDependencyProvider dependencyProvider) { }

    public async Task VerificateActor(IRequestModel payload, EndpointContext context, CancellationToken cancellationToken)
    {
        // Bu endpoint Ocelot'a eklenmediği için dışarıdan erişilemez.
        // Sadece HangfireScheduler tarafından iç ağdan çağrılır.
        await Task.CompletedTask;
    }

    public async Task VerificateDomain(IRequestModel payload, EndpointContext context, CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
    }
}
