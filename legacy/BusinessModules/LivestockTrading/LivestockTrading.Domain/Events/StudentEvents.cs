namespace LivestockTrading.Domain.Events;

/// <summary>
/// Öğrenci oluşturulduğunda tetiklenen event
/// </summary>
public class StudentCreatedEvent : IDomainEvent
{
    public Guid StudentId { get; set; }
    public string StudentNumber { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string Department { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime OccurredAt => CreatedAt;
}

/// <summary>
/// Öğrenci güncellendiğinde tetiklenen event
/// </summary>
public class StudentUpdatedEvent : IDomainEvent
{
    public Guid StudentId { get; set; }
    public string StudentNumber { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string Department { get; set; }
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public DateTime OccurredAt => UpdatedAt;
}

/// <summary>
/// Öğrenci silindiğinde tetiklenen event
/// </summary>
public class StudentDeletedEvent : IDomainEvent
{
    public Guid StudentId { get; set; }
    public string StudentNumber { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public DateTime DeletedAt { get; set; } = DateTime.UtcNow;

    public DateTime OccurredAt => DeletedAt;
}
