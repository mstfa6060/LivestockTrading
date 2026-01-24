namespace LivestockTrading.Application.RequestHandlers.ShippingCarriers.Commands.Update;

public class RequestModel : IRequestModel
{
	public Guid Id { get; set; }
	public string Name { get; set; }
	public string Code { get; set; }
	public string Website { get; set; }
	public string TrackingUrlTemplate { get; set; }
	public bool IsActive { get; set; }
	public string SupportedCountries { get; set; }
}

public class ResponseModel : IResponseModel
{
	public Guid Id { get; set; }
	public string Name { get; set; }
	public string Code { get; set; }
	public string Website { get; set; }
	public string TrackingUrlTemplate { get; set; }
	public bool IsActive { get; set; }
	public string SupportedCountries { get; set; }
	public DateTime? UpdatedAt { get; set; }
}
