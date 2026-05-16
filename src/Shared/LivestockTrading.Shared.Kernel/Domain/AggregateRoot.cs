namespace Shared.Domain;

/// <summary>
/// Aggregate root tabanı (AR = Entity'nin özel türü, Vernon). Domain event toplar.
/// Cross-AR referans yalnız ID (3d Kural 1) — navigation property base'de yok, derived'da da yasak.
/// _domainEvents bilinçli kilitsiz: bir AR instance tek UoW/thread içinde load-mutate-save edilir
/// (3d Kural 2 "bir command → bir AR"); eşzamanlı tek-AR mutasyonu desteklenen senaryo değil.
/// </summary>
public abstract class AggregateRoot : Entity
{
    private readonly List<IDomainEvent> _domainEvents = new();

    /// <summary>Toplanan event'ler. Infrastructure UoW SaveChanges sonrası dispatch + Clear eder.</summary>
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    protected void Raise(IDomainEvent domainEvent) => _domainEvents.Add(domainEvent);

    public void ClearDomainEvents() => _domainEvents.Clear();
}
