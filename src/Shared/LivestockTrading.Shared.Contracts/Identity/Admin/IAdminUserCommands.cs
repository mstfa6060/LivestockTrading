namespace Shared.Contracts.Identity.Admin;

using Shared.Results;

/// <summary>Admin kullanıcı yazma komutları (05-identity §5 literal 6 metot). Karar 3d cross-modül sync command.</summary>
public interface IAdminUserCommands
{
    Task<Result> SuspendAsync(Guid userId, string reason, Guid actorId, CancellationToken ct);
    Task<Result> ReactivateAsync(Guid userId, Guid actorId, CancellationToken ct);
    Task<Result> GrantRoleAsync(Guid userId, string role, Guid actorId, CancellationToken ct);
    Task<Result> RevokeRoleAsync(Guid userId, string role, Guid actorId, CancellationToken ct);
    Task<Result> ForceLogoutAsync(Guid userId, Guid actorId, CancellationToken ct);
    Task<Result> ImpersonateAsync(Guid targetUserId, Guid adminUserId, string justification, CancellationToken ct);
}
