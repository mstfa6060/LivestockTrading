namespace LivestockTrading.Application.RequestHandlers.TaxRates.Commands.Create;

public class RequestModel : IRequestModel
{
	public string CountryCode { get; set; }
	public string StateCode { get; set; }
	public string TaxName { get; set; }
	public double Rate { get; set; }
	public int Type { get; set; }
	public Guid? CategoryId { get; set; }
	public bool IsActive { get; set; } = true;
	public DateTime? ValidFrom { get; set; }
	public DateTime? ValidUntil { get; set; }
}

public class ResponseModel : IResponseModel
{
	public Guid Id { get; set; }
	public string CountryCode { get; set; }
	public string StateCode { get; set; }
	public string TaxName { get; set; }
	public double Rate { get; set; }
	public int Type { get; set; }
	public Guid? CategoryId { get; set; }
	public bool IsActive { get; set; }
	public DateTime? ValidFrom { get; set; }
	public DateTime? ValidUntil { get; set; }
	public DateTime CreatedAt { get; set; }
}
