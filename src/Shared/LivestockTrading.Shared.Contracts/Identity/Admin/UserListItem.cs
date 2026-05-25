namespace Shared.Contracts.Identity.Admin;

using Shared.Contracts.Identity;

/// <summary>Admin paneli kullanıcı liste projeksiyonu (cursor page). Email + display + status + AccountType.</summary>
public sealed record UserListItem(
    Guid UserId,
    string Email,
    string DisplayName,
    AccountType AccountType,
    UserStatus Status,
    DateTimeOffset CreatedAt);
