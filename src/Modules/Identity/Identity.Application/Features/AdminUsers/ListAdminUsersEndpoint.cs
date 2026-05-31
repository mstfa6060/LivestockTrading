using Microsoft.AspNetCore.Http;
using Shared.Contracts.Identity.Admin;

namespace LivestockTrading.Identity.Application.Features.AdminUsers;

/// <summary>
/// GET /admin/users — cursor list with optional filters (status, role, account_type,
/// search). UserListQuery [AsParameters] query binding (Catalog ListBrandsEndpoint
/// emsali). Backing IAdminUserReadService impl W4.3 Infrastructure scope (auth-inert
/// runtime, yapisal tam); 501-mapping host concern (deviations retro 19 emsali).
/// </summary>
public static class ListAdminUsersEndpoint
{
    public static async Task<IResult> Handle(
        [AsParameters] UserListQuery query,
        IAdminUserReadService readService,
        CancellationToken ct)
    {
        return Results.Ok(await readService.ListUsersAsync(query, ct));
    }
}
