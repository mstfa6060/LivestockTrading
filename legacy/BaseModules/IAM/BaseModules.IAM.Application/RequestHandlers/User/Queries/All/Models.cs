namespace BaseModules.IAM.Application.RequestHandlers.Users.Queries.All;

public class RequestModel : IRequestModel
{
	public XSorting Sorting { get; set; }
	public List<XFilterItem> Filters { get; set; }
	public XPageRequest PageRequest { get; set; }
}

public class ResponseModel : IResponseModel<Array>
{
	public Guid UserId { get; set; }
	public string UserName { get; set; }
	public string Email { get; set; }
	public string FullName { get; set; }
	public bool IsActive { get; set; }
	public bool IsAvailable { get; set; }
	public string PhoneNumber { get; set; }
	public string BucketId { get; set; }
	public List<string> Roles { get; set; } = new();
}
