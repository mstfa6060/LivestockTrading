namespace LivestockTrading.Application.RequestHandlers.Transporters.Queries.All;

public class RequestModel : IRequestModel
{
	public XSorting Sorting { get; set; }
	public List<XFilterItem> Filters { get; set; }
	public XPageRequest PageRequest { get; set; }
}

public class ResponseModel : IResponseModel<Array>
{
	public Guid Id { get; set; }
	public Guid UserId { get; set; }
	public string CompanyName { get; set; }
	public string ContactPerson { get; set; }
	public string Email { get; set; }
	public string Phone { get; set; }
	public string City { get; set; }
	public string CountryCode { get; set; }
	public bool IsVerified { get; set; }
	public bool IsActive { get; set; }
	public decimal? AverageRating { get; set; }
	public int TotalTransports { get; set; }
	public DateTime CreatedAt { get; set; }
}
