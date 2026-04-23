namespace LivestockTrading.Application.RequestHandlers.ProductViewHistories.Commands.Update;

public class RequestModel : IRequestModel
{
	public Guid Id { get; set; }
	public Guid UserId { get; set; }
	public Guid ProductId { get; set; }
	public string ViewSource { get; set; }
}

public class ResponseModel : IResponseModel
{
	public Guid Id { get; set; }
	public Guid UserId { get; set; }
	public Guid ProductId { get; set; }
	public DateTime ViewedAt { get; set; }
	public string ViewSource { get; set; }
	public DateTime? UpdatedAt { get; set; }
}
