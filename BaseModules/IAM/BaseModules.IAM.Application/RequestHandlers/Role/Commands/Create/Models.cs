
namespace BaseModules.IAM.Application.RequestHandlers.Roles.Commands.Create;

public class ResponseModel : IResponseModel
{
	public Guid Id { get; set; }
}

public class RequestModel : IRequestModel
{
	public Guid Id { get; set; }
	public string Name { get; set; }
}