namespace LivestockTrading.Application.RequestHandlers.AnimalInfos.Queries.All;

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
	public string BreedName { get; set; }
	public int Gender { get; set; }
	public int? AgeMonths { get; set; }
	public decimal? WeightKg { get; set; }
	public string Color { get; set; }
	public string TagNumber { get; set; }
	public int HealthStatus { get; set; }
	public int Purpose { get; set; }
	public DateTime CreatedAt { get; set; }
}
