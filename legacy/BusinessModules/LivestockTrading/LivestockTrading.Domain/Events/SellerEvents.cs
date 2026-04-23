namespace LivestockTrading.Domain.Events;

/// <summary>
/// Event fired when a new seller profile is created (PendingVerification).
/// Used to notify admin/moderator users about new sellers awaiting verification.
/// </summary>
public class SellerCreatedEvent : IDomainEvent
{
    public Guid SellerId { get; set; }
    public Guid UserId { get; set; }
    public string BusinessName { get; set; }
    public string BusinessType { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }

    /// <summary>Admin/Moderator user IDs to notify (resolved at publish time)</summary>
    public List<Guid> TargetAdminUserIds { get; set; } = new();

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime OccurredAt => CreatedAt;
}
