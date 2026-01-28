namespace LivestockTrading.Application.RequestHandlers.Transporters.Queries.Pick;

public class Verificator : IRequestVerificator
{
	public Verificator(ArfBlocksDependencyProvider dependencyProvider)
	{
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
