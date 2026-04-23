namespace Shared.Contracts.Integration;

/// <summary>
/// Modüller arası iletişim için kullanılan NATS JetStream integration event'leri.
/// Her modül kendi event'lerini bu interface'i implemente ederek tanımlar.
/// </summary>
public interface IIntegrationEvent
{
    Guid EventId { get; }
    DateTime OccurredAt { get; }
}
