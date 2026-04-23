namespace LivestockTrading.Application.RequestHandlers.ShippingZones.Queries.Detail;

public class RequestModel : IRequestModel
{
	public Guid Id { get; set; }
}

public class ResponseModel : IResponseModel
{
	public Guid Id { get; set; }
	public Guid? SellerId { get; set; }
	public string Name { get; set; }
	public string CountryCodes { get; set; }
	public bool IsActive { get; set; }
	public DateTime CreatedAt { get; set; }
	public DateTime? UpdatedAt { get; set; }
}
