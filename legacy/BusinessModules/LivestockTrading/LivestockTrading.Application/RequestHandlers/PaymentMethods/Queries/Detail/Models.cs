namespace LivestockTrading.Application.RequestHandlers.PaymentMethods.Queries.Detail;

public class RequestModel : IRequestModel
{
	public Guid Id { get; set; }
}

public class ResponseModel : IResponseModel
{
	public Guid Id { get; set; }
	public string Name { get; set; }
	public string Code { get; set; }
	public string Description { get; set; }
	public string IconUrl { get; set; }
	public bool RequiresManualVerification { get; set; }
	public bool IsActive { get; set; }
	public string SupportedCountries { get; set; }
	public string SupportedCurrencies { get; set; }
	public double? TransactionFeePercentage { get; set; }
	public double? FixedTransactionFee { get; set; }
	public DateTime CreatedAt { get; set; }
	public DateTime? UpdatedAt { get; set; }
}
