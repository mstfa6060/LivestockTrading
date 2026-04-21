namespace BaseModules.IAM.Application.RequestHandlers.UserPermissions.Queries.My;

public class RequestModel : IRequestModel
{
	// Boş - current user'ın yetkilerini döner
}

public class ResponseModel : IResponseModel
{
	public List<string> Permissions { get; set; }
	public List<ResourceModel> Resources { get; set; }
}

public class ResourceModel
{
	public string Namespace { get; set; }
	public string Name { get; set; }
	public string Title { get; set; }
}