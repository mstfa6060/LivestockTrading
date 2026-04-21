namespace LivestockTrading.Application.RequestHandlers.AppVersions.Queries.Check;

public class Verificator : IRequestVerificator
{
	public Verificator(ArfBlocksDependencyProvider dependencyProvider)
	{
	}

	// NOT: Bu endpoint KASITLI olarak PUBLIC'tir. Mobil/web uygulama login olmadan
	// once acilisinda cagirabilir, dolayisiyla burada rol kontrolu veya
	// authentication asserti yapilmaz. Ocelot gateway'inde de AuthenticationOptions
	// olmadan tanimlanmalidir.
	public async Task VerificateActor(IRequestModel payload, EndpointContext context, CancellationToken cancellationToken)
	{
		await Task.CompletedTask;
	}

	public async Task VerificateDomain(IRequestModel payload, EndpointContext context, CancellationToken cancellationToken)
	{
		await Task.CompletedTask;
	}
}
