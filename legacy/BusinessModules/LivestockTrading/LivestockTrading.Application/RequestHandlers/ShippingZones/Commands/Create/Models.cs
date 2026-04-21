namespace LivestockTrading.Application.RequestHandlers.ShippingZones.Commands.Create;

public class RequestModel : IRequestModel
{
	public Guid? SellerId { get; set; }
	public string Name { get; set; }
	public string CountryCodes { get; set; }
	public bool IsActive { get; set; } = true;
}

public class ResponseModel : IResponseModel
{
	public Guid Id { get; set; }
	public Guid? SellerId { get; set; }
	public string Name { get; set; }
	public string CountryCodes { get; set; }
	public bool IsActive { get; set; }
	public DateTime CreatedAt { get; set; }
}
