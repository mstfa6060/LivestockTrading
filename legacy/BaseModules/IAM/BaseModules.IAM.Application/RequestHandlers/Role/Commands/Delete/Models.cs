namespace BaseModules.IAM.Application.RequestHandlers.Roles.Commands.Delete;

public class RequestModel : IRequestModel
{
	public Guid Id { get; set; }
	public bool IsDeleted { get; set; }
}
public class ResponseModel : IResponseModel
{
	public Guid Id { get; set; }
	public bool IsDeleted { get; set; }
}