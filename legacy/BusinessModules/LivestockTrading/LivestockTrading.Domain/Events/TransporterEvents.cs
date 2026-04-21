namespace LivestockTrading.Domain.Events;

/// <summary>
/// Event fired when a new transporter profile is created (PendingVerification).
/// Used to notify admin/moderator users about new transporters awaiting verification.
/// </summary>
public class TransporterCreatedEvent : IDomainEvent
{
    public Guid TransporterId { get; set; }
    public Guid UserId { get; set; }
    public string CompanyName { get; set; }
    public string ContactPerson { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }

    /// <summary>Admin/Moderator user IDs to notify (resolved at publish time)</summary>
    public List<Guid> TargetAdminUserIds { get; set; } = new();

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime OccurredAt => CreatedAt;
}
