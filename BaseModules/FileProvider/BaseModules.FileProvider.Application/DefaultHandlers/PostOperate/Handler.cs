
namespace BaseModules.FileProvider.Application.DefaultHandlers.Operators.Commands.PostOperate;

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

		await _auditLogService.CreateLog(requestPayload.Endpoint, null, requestPayload.Response, context, AllModules.Definitions);
		//await ProcessData(requestPayload.Endpoint, requestPayload.Response, context, currentUserId, currentUserIp);

		// Map to Response Model
		var mappedResponseModel = mapper.MapToResponseModel();
		return ArfBlocksResults.Success(mappedResponseModel);
	}
}
