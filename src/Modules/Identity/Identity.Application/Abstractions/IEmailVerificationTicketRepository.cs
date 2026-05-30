using LivestockTrading.Identity.Domain.Aggregates;
using LivestockTrading.Identity.Domain.Enums;
using LivestockTrading.Identity.Domain.ValueObjects;

namespace LivestockTrading.Identity.Application.Abstractions;

/// <summary>
/// Thin repository port for EmailVerificationTicket aggregate root
/// (PhoneVerificationTicket emsali). Implementation in Identity.Infrastructure
/// (Wave 4 W4.3) wires EF Core. SaveChanges semantics handled by IUnitOfWork
/// pipeline filter, NOT this port. GetActiveByEmailAsync resolves the latest
/// non-consumed, non-expired ticket matching the email address + purpose
/// (PhoneVerificationTicketRepository.GetActiveByPhoneAsync purpose-discriminator
/// emsali, W4.2.D2-out cift-kanal Flow A) — handlers enforce attempt count and
/// TTL via the AR itself.
/// </summary>
public interface IEmailVerificationTicketRepository
{
    Task<EmailVerificationTicket?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<EmailVerificationTicket?> GetActiveByEmailAsync(EmailAddress email, EmailPurpose purpose, CancellationToken ct);
    Task AddAsync(EmailVerificationTicket ticket, CancellationToken ct);
    void Remove(EmailVerificationTicket ticket);
}
