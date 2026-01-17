namespace BaseModules.IAM.Application.RequestHandlers.Companies.Commands.Create;

public class RequestModel : IRequestModel
{
	public string Name { get; set; }
	public string TaxNumber { get; set; }
	public string Address { get; set; }
	public string Phone { get; set; }
}

public class ResponseModel : IResponseModel
{
	public Guid Id { get; set; } 
}
