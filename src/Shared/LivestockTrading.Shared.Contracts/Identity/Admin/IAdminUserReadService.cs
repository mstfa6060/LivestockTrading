namespace Shared.Contracts.Identity.Admin;

using Shared.Pagination;

/// <summary>Admin kullanıcı okuma (05-admin §5 Karar 3d stub — 05-identity §12 endpoint'lerinden türetildi).</summary>
public interface IAdminUserReadService
{
    Task<CursorPage<UserListItem>> ListUsersAsync(UserListQuery query, CancellationToken ct);
    Task<UserDetail?> GetUserByIdAsync(Guid userId, CancellationToken ct);
}
