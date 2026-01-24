namespace LivestockTrading.Application.RequestHandlers.Transporters.Commands.Create;

public class RequestModel : IRequestModel
{
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
	public string LicenseNumber { get; set; }
	public string TaxNumber { get; set; }
	public string InsuranceInfo { get; set; }
	public string FleetInfo { get; set; }
	public string ServiceRegions { get; set; }
	public string Specializations { get; set; }
	public bool IsActive { get; set; } = true;
	public string Website { get; set; }
	public string Certifications { get; set; }
	public string DocumentUrls { get; set; }
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
	public string LicenseNumber { get; set; }
	public string TaxNumber { get; set; }
	public string InsuranceInfo { get; set; }
	public string FleetInfo { get; set; }
	public string ServiceRegions { get; set; }
	public string Specializations { get; set; }
	public bool IsActive { get; set; }
	public string Website { get; set; }
	public string Certifications { get; set; }
	public string DocumentUrls { get; set; }
	public DateTime CreatedAt { get; set; }
}
