namespace Common.Contracts.PubSub.Models;

public class EventModelContract
{
	public Guid Id { get; set; }
	public DateTime PublishedAt { get; set; }
	public string Version { get; set; }
	public string ModuleName { get; set; }
	public string EventName { get; set; }
	// public string Payload { get; set; }

	// Payload
	public Guid ObjectId { get; set; }
	public Guid? ChildObjectId { get; set; }
	public Guid UserId { get; set; }
	public string UserMessage { get; set; }
	public bool IsInternalCall { get; set; }


}