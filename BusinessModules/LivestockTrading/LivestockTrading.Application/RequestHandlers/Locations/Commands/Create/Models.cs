namespace LivestockTrading.Application.RequestHandlers.Locations.Commands.Create;

public class RequestModel : IRequestModel
{
	public string Name { get; set; }
	public string AddressLine1 { get; set; }
	public string AddressLine2 { get; set; }
	public string City { get; set; }
	public string State { get; set; }
	public string PostalCode { get; set; }
	public string CountryCode { get; set; }
	public double? Latitude { get; set; }
	public double? Longitude { get; set; }
	public string Phone { get; set; }
	public string Email { get; set; }
	public int Type { get; set; }
	public bool IsActive { get; set; } = true;
	public Guid? UserId { get; set; }
}

public class ResponseModel : IResponseModel
{
	public Guid Id { get; set; }
	public string Name { get; set; }
	public string AddressLine1 { get; set; }
	public string AddressLine2 { get; set; }
	public string City { get; set; }
	public string State { get; set; }
	public string PostalCode { get; set; }
	public string CountryCode { get; set; }
	public double? Latitude { get; set; }
	public double? Longitude { get; set; }
	public string Phone { get; set; }
	public string Email { get; set; }
	public int Type { get; set; }
	public bool IsActive { get; set; }
	public Guid? UserId { get; set; }
	public DateTime CreatedAt { get; set; }
}
