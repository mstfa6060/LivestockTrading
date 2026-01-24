namespace LivestockTrading.Application.RequestHandlers.Conversations.Queries.Detail;

public class RequestModel : IRequestModel
{
	public Guid Id { get; set; }
}

public class ResponseModel : IResponseModel
{
	public Guid Id { get; set; }
	public Guid ParticipantUserId1 { get; set; }
	public Guid ParticipantUserId2 { get; set; }
	public Guid? ProductId { get; set; }
	public Guid? OrderId { get; set; }
	public string Subject { get; set; }
	public int Status { get; set; }
	public DateTime? LastMessageAt { get; set; }
	public DateTime CreatedAt { get; set; }
	public DateTime? UpdatedAt { get; set; }
}
