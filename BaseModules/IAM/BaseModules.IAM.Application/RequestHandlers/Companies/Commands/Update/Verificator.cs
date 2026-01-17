namespace BaseModules.IAM.Application.RequestHandlers.Companies.Commands.Update;

public class Verificator : IRequestVerificator
{
	public Verificator(ArfBlocksDependencyProvider _) { }

	public async Task VerificateActor(IRequestModel payload, EndpointContext context, CancellationToken cancellationToken)
	{
		await Task.CompletedTask;
	}

	public async Task VerificateDomain(IRequestModel payload, EndpointContext context, CancellationToken cancellationToken)
	{
		await Task.CompletedTask;
	}
}
