namespace LivestockTrading.Application.RequestHandlers.Sellers.Queries.All;

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
	public string BusinessName { get; set; }
	public string BusinessType { get; set; }
	public string Email { get; set; }
	public string Phone { get; set; }
	public bool IsVerified { get; set; }
	public bool IsActive { get; set; }
	public int Status { get; set; }
	public decimal? AverageRating { get; set; }
	public int TotalReviews { get; set; }
	public int TotalSales { get; set; }
	public DateTime CreatedAt { get; set; }
}
