namespace LivestockTrading.Application.RequestHandlers.ProductVideos.Commands.Update;

public class RequestModel : IRequestModel
{
	public Guid Id { get; set; }
	public string VideoUrl { get; set; }
	public string ThumbnailUrl { get; set; }
	public string Title { get; set; }
	public int DurationSeconds { get; set; }
	public int SortOrder { get; set; }
	public int Provider { get; set; }
}

public class ResponseModel : IResponseModel
{
	public Guid Id { get; set; }
	public Guid ProductId { get; set; }
	public string VideoUrl { get; set; }
	public string ThumbnailUrl { get; set; }
	public string Title { get; set; }
	public int DurationSeconds { get; set; }
	public int SortOrder { get; set; }
	public int Provider { get; set; }
	public DateTime? UpdatedAt { get; set; }
}
