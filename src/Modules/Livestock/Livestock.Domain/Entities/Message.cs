namespace Livestock.Domain.Entities;

public class Message : BaseEntity
{
    public Guid ConversationId { get; set; }
    public Guid SenderUserId { get; set; }
    public Guid RecipientUserId { get; set; }
    public string Content { get; set; } = string.Empty;
    public bool IsRead { get; set; }
    public DateTime? ReadAt { get; set; }
    public string? AttachmentUrl { get; set; }
    public string? AttachmentType { get; set; }

    public Conversation Conversation { get; set; } = null!;
}
