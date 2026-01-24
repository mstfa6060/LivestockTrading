namespace LivestockTrading.Application.RequestHandlers.ProductDocuments.Commands.Create;

public class RequestModel : IRequestModel
{
	public Guid ProductId { get; set; }
	public string DocumentUrl { get; set; }
	public string FileName { get; set; }
	public string Title { get; set; }
	public int Type { get; set; }
	public long FileSizeBytes { get; set; }
	public string MimeType { get; set; }
	public int SortOrder { get; set; }
}

public class ResponseModel : IResponseModel
{
	public Guid Id { get; set; }
	public Guid ProductId { get; set; }
	public string DocumentUrl { get; set; }
	public string FileName { get; set; }
	public string Title { get; set; }
	public int Type { get; set; }
	public long FileSizeBytes { get; set; }
	public string MimeType { get; set; }
	public int SortOrder { get; set; }
	public DateTime CreatedAt { get; set; }
}
