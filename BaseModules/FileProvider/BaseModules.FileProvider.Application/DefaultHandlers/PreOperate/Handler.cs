namespace BaseModules.FileProvider.Application.DefaultHandlers.Operators.Commands.PreOperate;

[InternalHandler]
public class Handler : IRequestHandler
{
	private readonly AuditLogService _auditLogService;

	public Handler(ArfBlocksDependencyProvider dependencyProvider, object dataAccess)
	{
		_auditLogService = dependencyProvider.GetInstance<AuditLogService>();
	}

	public async Task<ArfBlocksRequestResult> Handle(IRequestModel payload, EndpointContext context, CancellationToken cancellationToken)
	{
		var mapper = new Mapper();
		var requestPayload = (RequestModel)payload;

		await _auditLogService.CreateLog(requestPayload.Endpoint, requestPayload.Payload, null, context, AllModules.FileProvider);
		// await ProcessData(requestPayload.Endpoint, requestPayload.Payload, context, currentUserId, currentUserIp);

		// Map to Response Model
		var mappedResponseModel = mapper.MapToResponseModel();
		return ArfBlocksResults.Success(mappedResponseModel);
	}
}
