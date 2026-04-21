namespace LivestockTrading.Application.RequestHandlers.Products.Queries.MediaDetail;

public class RequestModel : IRequestModel
{
	public Guid ProductId { get; set; }
}

public class ResponseModel : IResponseModel
{
	public Guid ProductId { get; set; }
	public string MediaBucketId { get; set; }
	public string CoverImageFileId { get; set; }
}
