using Microsoft.AspNetCore.Http;
using Shared.Contracts.Identity.Admin;

namespace LivestockTrading.Identity.Application.Features.AdminUsers;

/// <summary>
/// GET /admin/users/{id} — user detail projection. Read service null donduregi
/// durumda 404 NOT_FOUND. Backing IAdminUserReadService impl W4.3 Infrastructure
/// scope (auth-inert runtime, yapisal tam).
/// </summary>
public static class GetAdminUserDetailEndpoint
{
    public static async Task<IResult> Handle(
        Guid id,
        IAdminUserReadService readService,
        CancellationToken ct)
    {
        var detail = await readService.GetUserByIdAsync(id, ct);
        return detail is null ? Results.NotFound() : Results.Ok(detail);
    }
}
