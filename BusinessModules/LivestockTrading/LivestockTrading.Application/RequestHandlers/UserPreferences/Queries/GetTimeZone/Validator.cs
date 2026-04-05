namespace LivestockTrading.Application.RequestHandlers.UserPreferences.Queries.GetTimeZone;

public class Validator : IRequestValidator
{
	public Validator(ArfBlocksDependencyProvider dependencyProvider)
	{
	}

	public void ValidateRequestModel(IRequestModel payload, EndpointContext context, CancellationToken cancellationToken)
	{
		var request = (RequestModel)payload;
		if (request.UserId == Guid.Empty)
			throw new ArfBlocksValidationException("UserId is required.");
	}

	public async Task ValidateDomain(IRequestModel payload, EndpointContext context, CancellationToken cancellationToken)
	{
		await Task.CompletedTask;
	}
}
