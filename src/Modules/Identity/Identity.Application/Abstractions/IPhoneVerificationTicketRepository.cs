using LivestockTrading.Identity.Domain.Aggregates;
using LivestockTrading.Identity.Domain.Enums;
using LivestockTrading.Identity.Domain.ValueObjects;

namespace LivestockTrading.Identity.Application.Abstractions;

/// <summary>
/// Thin repository port for PhoneVerificationTicket aggregate root (standalone AR,
/// plan-doc 05-identity §3). Implementation in Identity.Infrastructure (Wave 4 W4.3)
/// wires EF Core. SaveChanges semantics handled by IUnitOfWork pipeline filter, NOT
/// this port. GetActiveByPhoneAsync resolves the latest non-consumed, non-expired
/// ticket matching (Phone, Purpose) — handlers enforce attempt count and TTL.
/// </summary>
public interface IPhoneVerificationTicketRepository
{
    Task<PhoneVerificationTicket?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<PhoneVerificationTicket?> GetActiveByPhoneAsync(PhoneNumber phone, PhonePurpose purpose, CancellationToken ct);
    Task AddAsync(PhoneVerificationTicket ticket, CancellationToken ct);
    void Remove(PhoneVerificationTicket ticket);
}
