using Microsoft.AspNetCore.Http;
using Shared.Contracts.Identity.Admin;

namespace LivestockTrading.Identity.Application.Features.AdminUsers;

/// <summary>
/// GET /admin/users/{id}/sessions — active session list for target user (admin
/// inceleme). Read service null = user yok (404); empty list = user var ama
/// session yok. Backing IAdminUserReadService impl W4.3 Infrastructure scope.
/// </summary>
public static class GetAdminUserSessionsEndpoint
{
    public static async Task<IResult> Handle(
        Guid id,
        IAdminUserReadService readService,
        CancellationToken ct)
    {
        var sessions = await readService.GetSessionsByUserAsync(id, ct);
        return sessions is null ? Results.NotFound() : Results.Ok(sessions);
    }
}
