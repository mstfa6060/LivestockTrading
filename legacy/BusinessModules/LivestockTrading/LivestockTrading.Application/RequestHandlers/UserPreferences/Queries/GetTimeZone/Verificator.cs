namespace LivestockTrading.Application.RequestHandlers.UserPreferences.Queries.GetTimeZone;

public class Verificator : IRequestVerificator
{
	public Verificator(ArfBlocksDependencyProvider dependencyProvider)
	{
	}

	// Public: timezone bilgisi gizlilik acisindan dusuk riskli ve chat gibi
	// baskalarinin local saatini gostermek icin istemci tarafina acik kalmali.
	public async Task VerificateActor(IRequestModel payload, EndpointContext context, CancellationToken cancellationToken)
	{
		await Task.CompletedTask;
	}

	public async Task VerificateDomain(IRequestModel payload, EndpointContext context, CancellationToken cancellationToken)
	{
		await Task.CompletedTask;
	}
}
