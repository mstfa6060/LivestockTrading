namespace LivestockTrading.Application.RequestHandlers.ProductReviews.Commands.Create;

public class RequestModel : IRequestModel
{
	public Guid ProductId { get; set; }
	public Guid UserId { get; set; }
	public Guid? OrderId { get; set; }
	public int Rating { get; set; }
	public string Title { get; set; }
	public string Comment { get; set; }
	public bool IsVerifiedPurchase { get; set; }
	public string ImageUrls { get; set; }
	public string VideoUrls { get; set; }
}

public class ResponseModel : IResponseModel
{
	public Guid Id { get; set; }
	public Guid ProductId { get; set; }
	public Guid UserId { get; set; }
	public Guid? OrderId { get; set; }
	public int Rating { get; set; }
	public string Title { get; set; }
	public string Comment { get; set; }
	public bool IsVerifiedPurchase { get; set; }
	public string ImageUrls { get; set; }
	public string VideoUrls { get; set; }
	public DateTime CreatedAt { get; set; }
}
