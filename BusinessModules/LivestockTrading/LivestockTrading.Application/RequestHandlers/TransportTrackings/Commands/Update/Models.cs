namespace LivestockTrading.Application.RequestHandlers.TransportTrackings.Commands.Update;

public class RequestModel : IRequestModel
{
	public Guid Id { get; set; }
	public Guid TransportRequestId { get; set; }
	public double? Latitude { get; set; }
	public double? Longitude { get; set; }
	public string LocationDescription { get; set; }
	public int Status { get; set; }
	public string StatusDescription { get; set; }
	public DateTime RecordedAt { get; set; }
	public string Notes { get; set; }
	public string PhotoUrls { get; set; }
}

public class ResponseModel : IResponseModel
{
	public Guid Id { get; set; }
	public Guid TransportRequestId { get; set; }
	public double? Latitude { get; set; }
	public double? Longitude { get; set; }
	public string LocationDescription { get; set; }
	public int Status { get; set; }
	public string StatusDescription { get; set; }
	public DateTime RecordedAt { get; set; }
	public string Notes { get; set; }
	public string PhotoUrls { get; set; }
	public DateTime CreatedAt { get; set; }
	public DateTime? UpdatedAt { get; set; }
}
