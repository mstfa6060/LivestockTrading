using LivestockTrading.Identity.Domain.ValueObjects;

namespace LivestockTrading.Identity.Application.Abstractions;

/// <summary>
/// Application port for password hashing operations. Concrete implementation lives
/// in Identity.Infrastructure (Wave 4 W4.3) and uses PBKDF2 in a format compatible
/// with Microsoft.AspNetCore.Identity.PasswordHasher (versioned hash, iteration
/// upgrade-able without DB migration). Application orchestrators consume this port
/// for register/change-password (Hash) and login (Verify) flows.
/// </summary>
public interface IPasswordHasher
{
    HashedPassword Hash(string rawPassword);
    bool Verify(HashedPassword hashed, string rawPassword);
}
