namespace LivestockTrading.Application.RequestHandlers.Conversations.Queries.All;

public class Verificator : IRequestVerificator
{
	public Verificator(ArfBlocksDependencyProvider dependencyProvider)
	{
	}

	public async Task VerificateActor(IRequestModel payload, EndpointContext context, CancellationToken cancellationToken)
	{
		// JWT zaten Gateway'de dogrulaniyor; messaging icin rol kisitlamasi yok.
		// Kullanici izolasyonu DataAccess.All filter'inda (currentUserId bazli ParticipantUserId1/2).
		await Task.CompletedTask;
	}

	public async Task VerificateDomain(IRequestModel payload, EndpointContext context, CancellationToken cancellationToken)
	{
		await Task.CompletedTask;
	}
}
