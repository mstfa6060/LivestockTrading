using Common.Definitions.Domain.Errors;

namespace BaseModules.IAM.Application.EventHandlers.Mails.Queries.SendForgotPassword;

[InternalHandler]
[EventHandler]
public class Handler : IRequestHandler
{
	private readonly DataAccess _dataAccess;

	public Handler(ArfBlocksDependencyProvider dependencyProvider, object dataAccess)
	{
		_dataAccess = (DataAccess)dataAccess;
	}

	public async Task<ArfBlocksRequestResult> Handle(IRequestModel payload, EndpointContext context, CancellationToken cancellationToken)
	{
		var request = (RequestModel)payload;
		var user = await _dataAccess.GetByEmail(request.Email);

		if (user is null)
			throw new ArfBlocksValidationException(DomainErrors.UserErrors.UserNotFound);

		var mapper = new Mapper();
		var response = mapper.MapToResponse(user);
		return ArfBlocksResults.Success(response);
	}
}
