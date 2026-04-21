namespace BaseModules.FileProvider.Application.EventHandlers.Files.Commands.Approve;

public class ResponseModel : EventHandlerResponseModel, IResponseModel
{
}

public class RequestModel : IRequestModel
{
	public string BucketId { get; set; }
	public Guid UserId { get; set; }
}
