namespace BaseModules.IAM.Application.RequestHandlers.User.Commands.Delete;

public class RequestModel : IRequestModel
{
	public Guid UserId { get; set; }
	public bool IsDeleted { get; set; }
}

public class ResponseModel : IResponseModel
{
	public Guid Id { get; set; }
	public bool IsDeleted { get; set; }
}
