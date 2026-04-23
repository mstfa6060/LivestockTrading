namespace LivestockTrading.Application.RequestHandlers.Currencies.Commands.Create;

public class RequestModel : IRequestModel
{
	public string Code { get; set; }
	public string Symbol { get; set; }
	public string Name { get; set; }
	public double ExchangeRateToUSD { get; set; }
	public bool IsActive { get; set; } = true;
}

public class ResponseModel : IResponseModel
{
	public Guid Id { get; set; }
	public string Code { get; set; }
	public string Symbol { get; set; }
	public string Name { get; set; }
	public double ExchangeRateToUSD { get; set; }
	public DateTime LastUpdated { get; set; }
	public bool IsActive { get; set; }
	public DateTime CreatedAt { get; set; }
}
