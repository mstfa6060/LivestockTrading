namespace LivestockTrading.Application.RequestHandlers.Conversations.Queries.Pick;

public class Validator : IRequestValidator
{
	public Validator(ArfBlocksDependencyProvider dependencyProvider)
	{
	}

	public void ValidateRequestModel(IRequestModel payload, EndpointContext context, CancellationToken cancellationToken)
	{
	}

	public async Task ValidateDomain(IRequestModel payload, EndpointContext context, CancellationToken cancellationToken)
	{
		await Task.CompletedTask;
	}
}
