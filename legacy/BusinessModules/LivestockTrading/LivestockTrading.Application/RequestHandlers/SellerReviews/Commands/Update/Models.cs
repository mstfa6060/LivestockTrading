namespace LivestockTrading.Application.RequestHandlers.SellerReviews.Commands.Update;

public class RequestModel : IRequestModel
{
	public Guid Id { get; set; }
	public Guid SellerId { get; set; }
	public Guid UserId { get; set; }
	public Guid? OrderId { get; set; }
	public int OverallRating { get; set; }
	public int CommunicationRating { get; set; }
	public int ShippingSpeedRating { get; set; }
	public int ProductQualityRating { get; set; }
	public string Title { get; set; }
	public string Comment { get; set; }
	public bool IsVerifiedPurchase { get; set; }
}

public class ResponseModel : IResponseModel
{
	public Guid Id { get; set; }
	public Guid SellerId { get; set; }
	public Guid UserId { get; set; }
	public Guid? OrderId { get; set; }
	public int OverallRating { get; set; }
	public int CommunicationRating { get; set; }
	public int ShippingSpeedRating { get; set; }
	public int ProductQualityRating { get; set; }
	public string Title { get; set; }
	public string Comment { get; set; }
	public bool IsVerifiedPurchase { get; set; }
	public DateTime? UpdatedAt { get; set; }
}
