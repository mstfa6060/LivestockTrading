namespace LivestockTrading.Application.RequestHandlers.ProductReviews.Queries.Detail;

public class RequestModel : IRequestModel
{
	public Guid Id { get; set; }
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
	public bool IsApproved { get; set; }
	public DateTime? ApprovedAt { get; set; }
	public Guid? ApprovedByUserId { get; set; }
	public int HelpfulCount { get; set; }
	public int NotHelpfulCount { get; set; }
	public string SellerResponse { get; set; }
	public DateTime? SellerRespondedAt { get; set; }
	public string ImageUrls { get; set; }
	public string VideoUrls { get; set; }
	public DateTime CreatedAt { get; set; }
	public DateTime? UpdatedAt { get; set; }
}
