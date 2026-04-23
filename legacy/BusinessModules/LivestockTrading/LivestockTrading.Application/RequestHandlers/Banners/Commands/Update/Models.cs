namespace LivestockTrading.Application.RequestHandlers.Banners.Commands.Update;

public class RequestModel : IRequestModel
{
	public Guid Id { get; set; }
	public string Title { get; set; }
	public string Description { get; set; }
	public string ImageUrl { get; set; }
	public string TargetUrl { get; set; }
	public int Position { get; set; }
	public DateTime StartDate { get; set; }
	public DateTime EndDate { get; set; }
	public bool IsActive { get; set; }
	public int DisplayOrder { get; set; }
}

public class ResponseModel : IResponseModel
{
	public Guid Id { get; set; }
	public string Title { get; set; }
	public string Description { get; set; }
	public string ImageUrl { get; set; }
	public string TargetUrl { get; set; }
	public int Position { get; set; }
	public DateTime StartDate { get; set; }
	public DateTime EndDate { get; set; }
	public bool IsActive { get; set; }
	public int DisplayOrder { get; set; }
	public DateTime? UpdatedAt { get; set; }
}
