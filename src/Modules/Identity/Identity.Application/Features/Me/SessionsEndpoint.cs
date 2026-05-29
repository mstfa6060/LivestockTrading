using LivestockTrading.Identity.Application.Abstractions;
using LivestockTrading.Identity.Application.Common;
using Microsoft.AspNetCore.Http;

namespace LivestockTrading.Identity.Application.Features.Me;

/// <summary>
/// GET /identity/users/me/sessions — active session list, endpoint-direct port
/// consumption. SessionInfo.IsCurrent stays false in Faz 1 (W4.4 host-auth
/// wiring will surface the active refresh-token correlation).
/// </summary>
public static class SessionsEndpoint
{
    public static async Task<IResult> Handle(
        HttpContext http,
        IMeReadService readService,
        CancellationToken ct)
    {
        var userId = http.GetUserId();
        var sessions = await readService.GetMySessionsAsync(userId, ct);
        return Results.Ok(sessions);
    }
}
