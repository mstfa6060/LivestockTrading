namespace LivestockTrading.Application.RequestHandlers.Messages.Commands.SendTypingIndicator;

public class Verificator : IRequestVerificator
{
	public Verificator(ArfBlocksDependencyProvider dependencyProvider)
	{
	}

	public Task VerificateActor(IRequestModel payload, EndpointContext context, CancellationToken cancellationToken)
	{
		return Task.CompletedTask;
	}

	public Task VerificateDomain(IRequestModel payload, EndpointContext context, CancellationToken cancellationToken)
	{
		return Task.CompletedTask;
	}
}
