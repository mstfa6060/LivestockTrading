using Livestock.Domain.Enums;

namespace Livestock.Domain.Entities;

public class Conversation : BaseEntity
{
    public Guid InitiatorUserId { get; set; }
    public Guid RecipientUserId { get; set; }
    public Guid? ProductId { get; set; }
    public ConversationStatus Status { get; set; } = ConversationStatus.Active;
    public DateTime? LastMessageAt { get; set; }
    public int UnreadCountInitiator { get; set; }
    public int UnreadCountRecipient { get; set; }

    public ICollection<Message> Messages { get; set; } = [];
}
