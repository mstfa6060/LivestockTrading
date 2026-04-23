namespace LivestockTrading.Application.RequestHandlers.Languages.Queries.All;

public class RequestModel : IRequestModel
{
	public XSorting Sorting { get; set; }
	public List<XFilterItem> Filters { get; set; }
	public XPageRequest PageRequest { get; set; }
}

public class ResponseModel : IResponseModel<Array>
{
	public Guid Id { get; set; }
	public string Code { get; set; }
	public string Name { get; set; }
	public string NativeName { get; set; }
	public bool IsRightToLeft { get; set; }
	public bool IsActive { get; set; }
	public bool IsDefault { get; set; }
	public int SortOrder { get; set; }
	public string FlagIconUrl { get; set; }
	public DateTime CreatedAt { get; set; }
}
