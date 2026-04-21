namespace LivestockTrading.Application.RequestHandlers.Notifications.Commands.Update;

public class RequestModel : IRequestModel
{
	public Guid Id { get; set; }
	public string Title { get; set; }
	public string Message { get; set; }
	public int Type { get; set; }
	public string ActionUrl { get; set; }
	public string ActionData { get; set; }
	public bool IsRead { get; set; }
}

public class ResponseModel : IResponseModel
{
	public Guid Id { get; set; }
	public Guid UserId { get; set; }
	public string Title { get; set; }
	public string Message { get; set; }
	public int Type { get; set; }
	public string ActionUrl { get; set; }
	public string ActionData { get; set; }
	public bool IsRead { get; set; }
	public DateTime? ReadAt { get; set; }
	public DateTime? UpdatedAt { get; set; }
}
