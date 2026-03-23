namespace LivestockTrading.Application.RequestHandlers.ProductReports.Commands.Update;

public class RequestModel : IRequestModel
{
	public Guid Id { get; set; }
	public int Status { get; set; }
	public string AdminNote { get; set; }
}

public class ResponseModel : IResponseModel
{
	public Guid Id { get; set; }
	public int Status { get; set; }
	public string AdminNote { get; set; }
	public Guid? ReviewedByUserId { get; set; }
	public DateTime? ReviewedAt { get; set; }
	public bool Success { get; set; }
}
