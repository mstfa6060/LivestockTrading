namespace LivestockTrading.Identity.Application.Features.Me;

/// <summary>
/// DELETE /identity/users/me/sessions/{sessionId} — revoke a single session
/// (refresh token). SessionId maps to RefreshToken.Id; users can only revoke
/// sessions on their own account.
/// </summary>
public sealed record RevokeSessionCommand(Guid UserId, Guid SessionId);
