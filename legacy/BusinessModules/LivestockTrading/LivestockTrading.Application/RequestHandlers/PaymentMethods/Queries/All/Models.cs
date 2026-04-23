namespace LivestockTrading.Application.RequestHandlers.PaymentMethods.Queries.All;

public class RequestModel : IRequestModel
{
	public XSorting Sorting { get; set; }
	public List<XFilterItem> Filters { get; set; }
	public XPageRequest PageRequest { get; set; }
}

public class ResponseModel : IResponseModel<Array>
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
}
