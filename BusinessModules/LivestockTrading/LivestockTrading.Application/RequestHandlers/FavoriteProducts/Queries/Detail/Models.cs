namespace LivestockTrading.Application.RequestHandlers.FavoriteProducts.Queries.Detail;

public class RequestModel : IRequestModel
{
	public Guid Id { get; set; }
}

public class ResponseModel : IResponseModel
{
	public Guid Id { get; set; }
	public Guid UserId { get; set; }
	public Guid ProductId { get; set; }
	public DateTime AddedAt { get; set; }
	public DateTime CreatedAt { get; set; }
}
