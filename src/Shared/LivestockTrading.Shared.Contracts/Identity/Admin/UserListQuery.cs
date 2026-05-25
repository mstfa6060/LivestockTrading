namespace Shared.Contracts.Identity.Admin;

using Shared.Contracts.Identity;

/// <summary>Admin user list query (cursor pagination, optional filters). AccountType + Status + Role + free-text Search. PageSize default 50 (Catalog BrandListFilter emsali).</summary>
public sealed record UserListQuery(
    AccountType? AccountType,
    UserStatus? Status,
    string? Role,
    string? Search,
    string? Cursor,
    int PageSize = 50);
