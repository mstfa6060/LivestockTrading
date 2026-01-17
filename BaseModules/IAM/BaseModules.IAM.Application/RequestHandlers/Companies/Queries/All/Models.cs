namespace BaseModules.IAM.Application.RequestHandlers.Companies.Queries.All;


public class ResponseModel : IResponseModel<Array>
{
	public Guid Id { get; set; }
	public string Name { get; set; }
	public string TaxNumber { get; set; }
	public string Address { get; set; }
	public string Phone { get; set; }
}

public class RequestModel : IRequestModel
{
	public XSorting Sorting { get; set; }
	public List<XFilterItem> Filters { get; set; }
	public XPageRequest PageRequest { get; set; }
}
