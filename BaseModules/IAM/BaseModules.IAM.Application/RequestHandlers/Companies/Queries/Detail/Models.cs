namespace BaseModules.IAM.Application.RequestHandlers.Companies.Queries.Detail;

public class RequestModel : IRequestModel
{
	public Guid Id { get; set; }
}

public class ResponseModel : IResponseModel
{
	public Guid Id { get; set; }
	public string Name { get; set; }
	public string TaxNumber { get; set; }
	public string Address { get; set; }
	public string Phone { get; set; }
}
