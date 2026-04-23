namespace LivestockTrading.Application.RequestHandlers.Farms.Queries.Detail;

public class RequestModel : IRequestModel
{
	public Guid Id { get; set; }
}

public class ResponseModel : IResponseModel
{
	public Guid Id { get; set; }
	public string Name { get; set; }
	public string Description { get; set; }
	public string RegistrationNumber { get; set; }
	public Guid SellerId { get; set; }
	public string SellerBusinessName { get; set; }
	public Guid LocationId { get; set; }
	public string LocationName { get; set; }
	public int Type { get; set; }
	public double? TotalAreaHectares { get; set; }
	public double? CultivatedAreaHectares { get; set; }
	public string Certifications { get; set; }
	public bool IsOrganic { get; set; }
	public string ImageUrls { get; set; }
	public string VideoUrl { get; set; }
	public bool IsActive { get; set; }
	public bool IsVerified { get; set; }
	public DateTime CreatedAt { get; set; }
	public DateTime? UpdatedAt { get; set; }
}
