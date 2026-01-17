namespace BaseModules.IAM.Application.RequestHandlers.Companies.Commands.Update;

public class RequestModel : IRequestModel
{
	public Guid Id { get; set; }
	public string Name { get; set; }
	public string TaxNumber { get; set; }
	public string Address { get; set; }
	public string Phone { get; set; }
}

public class ResponseModel : IResponseModel
{
	public Guid Id { get; set; }
}
