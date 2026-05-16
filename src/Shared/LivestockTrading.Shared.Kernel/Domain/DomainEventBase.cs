namespace Shared.Domain;

/// <summary>
/// Domain event taban kaydı (D kararı). Concrete event'ler bundan türer:
/// <c>public sealed record CategoryCreated(int CategoryId, string Slug) : DomainEventBase;</c>
/// abstract record → value equality + immutable + sıfır boilerplate (80 event DRY).
/// </summary>
public abstract record DomainEventBase : IDomainEvent
{
    public DateTimeOffset OccurredOn { get; } = DateTimeOffset.UtcNow;
}
