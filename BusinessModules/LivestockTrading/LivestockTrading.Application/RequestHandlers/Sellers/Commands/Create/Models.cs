namespace LivestockTrading.Application.RequestHandlers.Sellers.Commands.Create;

public class RequestModel : IRequestModel
{
	public Guid UserId { get; set; }
	public string BusinessName { get; set; }
	public string BusinessType { get; set; }
	public string TaxNumber { get; set; }
	public string RegistrationNumber { get; set; }
	public string Description { get; set; }
	public string LogoUrl { get; set; }
	public string BannerUrl { get; set; }
	public string Email { get; set; }
	public string Phone { get; set; }
	public string Website { get; set; }
	public bool IsActive { get; set; } = true;
	public int Status { get; set; }
	public string BusinessHours { get; set; }
	public string AcceptedPaymentMethods { get; set; }
	public string ReturnPolicy { get; set; }
	public string ShippingPolicy { get; set; }
	public string SocialMediaLinks { get; set; }
}

public class ResponseModel : IResponseModel
{
	public Guid Id { get; set; }
	public Guid UserId { get; set; }
	public string BusinessName { get; set; }
	public string BusinessType { get; set; }
	public string TaxNumber { get; set; }
	public string RegistrationNumber { get; set; }
	public string Description { get; set; }
	public string LogoUrl { get; set; }
	public string BannerUrl { get; set; }
	public string Email { get; set; }
	public string Phone { get; set; }
	public string Website { get; set; }
	public bool IsVerified { get; set; }
	public bool IsActive { get; set; }
	public int Status { get; set; }
	public string BusinessHours { get; set; }
	public string AcceptedPaymentMethods { get; set; }
	public string ReturnPolicy { get; set; }
	public string ShippingPolicy { get; set; }
	public string SocialMediaLinks { get; set; }
	public DateTime CreatedAt { get; set; }
}
