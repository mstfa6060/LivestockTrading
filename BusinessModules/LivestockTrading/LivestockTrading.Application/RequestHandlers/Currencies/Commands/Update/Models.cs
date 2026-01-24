namespace LivestockTrading.Application.RequestHandlers.Currencies.Commands.Update;

public class RequestModel : IRequestModel
{
	public Guid Id { get; set; }
	public string Code { get; set; }
	public string Symbol { get; set; }
	public string Name { get; set; }
	public decimal ExchangeRateToUSD { get; set; }
	public bool IsActive { get; set; }
}

public class ResponseModel : IResponseModel
{
	public Guid Id { get; set; }
	public string Code { get; set; }
	public string Symbol { get; set; }
	public string Name { get; set; }
	public decimal ExchangeRateToUSD { get; set; }
	public DateTime LastUpdated { get; set; }
	public bool IsActive { get; set; }
	public DateTime? UpdatedAt { get; set; }
}
