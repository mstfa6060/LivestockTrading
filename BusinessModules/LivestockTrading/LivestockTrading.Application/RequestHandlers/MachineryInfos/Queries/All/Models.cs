namespace LivestockTrading.Application.RequestHandlers.MachineryInfos.Queries.All;

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
	public string Model { get; set; }
	public int? YearOfManufacture { get; set; }
	public decimal? PowerHp { get; set; }
	public int? HoursUsed { get; set; }
	public bool HasWarranty { get; set; }
	public DateTime CreatedAt { get; set; }
}
