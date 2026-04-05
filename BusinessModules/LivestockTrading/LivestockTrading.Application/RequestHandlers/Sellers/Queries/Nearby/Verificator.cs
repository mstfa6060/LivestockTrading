namespace LivestockTrading.Application.RequestHandlers.Sellers.Queries.Nearby;

public class Verificator : IRequestVerificator
{
	public Verificator(ArfBlocksDependencyProvider dependencyProvider)
	{
	}

	// Nearby saticilar public: kullanici giris yapmadan da cagirabilir.
	public async Task VerificateActor(IRequestModel payload, EndpointContext context, CancellationToken cancellationToken)
	{
		await Task.CompletedTask;
	}

	public async Task VerificateDomain(IRequestModel payload, EndpointContext context, CancellationToken cancellationToken)
	{
		await Task.CompletedTask;
	}
}
