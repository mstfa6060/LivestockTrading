namespace LivestockTrading.Application.RequestHandlers.Conversations.Commands.Update;

public class Handler : IRequestHandler
{
	private readonly DataAccess _dataAccessLayer;

	public Handler(ArfBlocksDependencyProvider dependencyProvider, object dataAccess)
	{
		_dataAccessLayer = (DataAccess)dataAccess;
	}

	public async Task<ArfBlocksRequestResult> Handle(IRequestModel payload, EndpointContext context, CancellationToken cancellationToken)
	{
		var request = (RequestModel)payload;
		var mapper = new Mapper();

		var conversation = await _dataAccessLayer.GetConversationById(request.Id);

		if (conversation == null)
			throw new ArfBlocksValidationException(
				ErrorCodeGenerator.GetErrorCode(() => LivestockTradingDomainErrors.CommonErrors.IdNotValid));

		mapper.MapToEntity(request, conversation);

		await _dataAccessLayer.SaveChanges();

		var response = mapper.MapToResponse(conversation);
		return ArfBlocksResults.Success(response);
	}
}
