namespace BaseModules.FileProvider.Application.DefaultHandlers.Operators.Commands.PostOperate;

public class ResponseModel : IResponseModel
{
}

public class RequestModel : IRequestModel
{
	public EndpointModel Endpoint { get; set; }
	public ArfBlocksRequestResult Response { get; set; }
}
