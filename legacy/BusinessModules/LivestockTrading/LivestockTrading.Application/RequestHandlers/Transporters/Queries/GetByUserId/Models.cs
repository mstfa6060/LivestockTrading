namespace LivestockTrading.Application.RequestHandlers.Transporters.Queries.GetByUserId;

public class RequestModel : IRequestModel
{
	public Guid UserId { get; set; }
}

public class ResponseModel : IResponseModel
{
	public Guid Id { get; set; }
	public Guid UserId { get; set; }
	public string CompanyName { get; set; }
	public string ContactPerson { get; set; }
	public string Email { get; set; }
	public string Phone { get; set; }
	public string Address { get; set; }
	public string City { get; set; }
	public string CountryCode { get; set; }
	public string LogoUrl { get; set; }
	public string Description { get; set; }
	public string ServiceRegions { get; set; }
	public string Specializations { get; set; }
	public bool IsVerified { get; set; }
	public bool IsActive { get; set; }
	public int Status { get; set; }
	public double? AverageRating { get; set; }
	public int TotalTransports { get; set; }
	public int CompletedTransports { get; set; }
	public string Website { get; set; }
	public DateTime CreatedAt { get; set; }
}
