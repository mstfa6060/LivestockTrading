namespace BaseModules.FileProvider.Application.DefaultHandlers.Operators.Commands.PreOperate;

public class ResponseModel : IResponseModel
{
}

public class RequestModel : IRequestModel
{
	public EndpointModel Endpoint { get; set; }
	public IRequestModel Payload { get; set; }
}
