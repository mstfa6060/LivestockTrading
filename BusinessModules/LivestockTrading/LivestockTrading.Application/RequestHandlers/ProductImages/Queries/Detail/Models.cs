namespace LivestockTrading.Application.RequestHandlers.ProductImages.Queries.Detail;

public class RequestModel : IRequestModel
{
	public Guid Id { get; set; }
}

public class ResponseModel : IResponseModel
{
	public Guid Id { get; set; }
	public Guid ProductId { get; set; }
	public string ImageUrl { get; set; }
	public string ThumbnailUrl { get; set; }
	public string AltText { get; set; }
	public int SortOrder { get; set; }
	public bool IsPrimary { get; set; }
	public DateTime CreatedAt { get; set; }
	public DateTime? UpdatedAt { get; set; }
}
