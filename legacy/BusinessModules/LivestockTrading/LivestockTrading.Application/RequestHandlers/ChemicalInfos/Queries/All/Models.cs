namespace LivestockTrading.Application.RequestHandlers.ChemicalInfos.Queries.All;

public class RequestModel : IRequestModel
{
	public XSorting Sorting { get; set; }
	public List<XFilterItem> Filters { get; set; }
	public XPageRequest PageRequest { get; set; }
}

public class ResponseModel : IResponseModel<Array>
{
	public Guid Id { get; set; }
	public Guid ProductId { get; set; }
	public int Type { get; set; }
	public string SubType { get; set; }
	public string RegistrationNumber { get; set; }
	public int ToxicityLevel { get; set; }
	public bool IsOrganic { get; set; }
	public DateTime? ExpiryDate { get; set; }
	public DateTime CreatedAt { get; set; }
}
