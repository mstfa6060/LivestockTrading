namespace LivestockTrading.Application.RequestHandlers.ShippingRates.Queries.Pick;

public class RequestModel : IRequestModel
{
	public List<Guid> SelectedIds { get; set; }
	public string Keyword { get; set; }
	public int Limit { get; set; } = 10;
}

public class ResponseModel : IResponseModel<Array>
{
	public Guid Id { get; set; }
	public decimal ShippingCost { get; set; }
	public string Currency { get; set; }
}
