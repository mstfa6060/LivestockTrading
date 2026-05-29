using LivestockTrading.Identity.Domain.Aggregates;
using LivestockTrading.Identity.Domain.ValueObjects;

namespace LivestockTrading.Identity.Application.Abstractions;

/// <summary>
/// Thin repository port for User aggregate root.
/// Implementation in Identity.Infrastructure (Wave 4 W4.3) wires EF Core.
/// SaveChanges semantics handled by IUnitOfWork pipeline filter, NOT this port.
/// Lookup methods cover 3-method login discriminator (email/phone/nationalId)
/// and refresh-token rotation (TokenHash). Exists* variants are used by
/// registration pre-checks to surface CONFLICT_* errors before aggregate
/// construction.
/// </summary>
public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<User?> GetByEmailAsync(EmailAddress email, CancellationToken ct);
    Task<User?> GetByPhoneAsync(PhoneNumber phone, CancellationToken ct);
    Task<User?> GetByNationalIdAsync(NationalId nationalId, CancellationToken ct);
    Task<User?> GetByRefreshTokenHashAsync(string tokenHash, CancellationToken ct);
    Task<bool> ExistsByEmailAsync(EmailAddress email, CancellationToken ct);
    Task<bool> ExistsByPhoneAsync(PhoneNumber phone, CancellationToken ct);
    Task<bool> ExistsByNationalIdAsync(NationalId nationalId, CancellationToken ct);
    Task AddAsync(User user, CancellationToken ct);
    void Remove(User user);
}
