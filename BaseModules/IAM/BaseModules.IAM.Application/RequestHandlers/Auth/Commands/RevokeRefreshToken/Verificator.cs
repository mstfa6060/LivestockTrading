namespace BaseModules.IAM.Application.RequestHandlers.Auth.Commands.RevokeRefreshToken;

public class Verificator : IRequestVerificator
{
	public Verificator(ArfBlocksDependencyProvider _) { }

	public Task VerificateActor(IRequestModel payload, EndpointContext context, CancellationToken cancellationToken)
	{
		return Task.CompletedTask;
	}

	public Task VerificateDomain(IRequestModel payload, EndpointContext context, CancellationToken cancellationToken)
	{
		return Task.CompletedTask;
	}
}
