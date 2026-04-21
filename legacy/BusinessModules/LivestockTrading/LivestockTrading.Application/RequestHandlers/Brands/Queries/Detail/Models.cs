namespace LivestockTrading.Application.RequestHandlers.Brands.Queries.Detail;

public class RequestModel : IRequestModel
{
	public Guid Id { get; set; }
}

public class ResponseModel : IResponseModel
{
	public Guid Id { get; set; }
	public string Name { get; set; }
	public string Slug { get; set; }
	public string Description { get; set; }
	public string LogoUrl { get; set; }
	public string Website { get; set; }
	public string Email { get; set; }
	public string Phone { get; set; }
	public string CountryCode { get; set; }
	public bool IsActive { get; set; }
	public bool IsVerified { get; set; }
	public int ProductCount { get; set; }
	public DateTime CreatedAt { get; set; }
	public DateTime? UpdatedAt { get; set; }
}
