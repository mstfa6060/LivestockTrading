using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace LivestockTrading.Catalog.Application.Common;

public static class ClaimsPrincipalExtensions
{
    /// <summary>
    /// Resolves admin actor ID from HttpContext claims.
    /// Identity host-auth henuz yapilandirilmadi — bu helper Wave N (Identity wave)
    /// implement edildiginde refine edilecek veya Shared.Web modulune ekstrakte edilecek.
    /// </summary>
    public static Guid GetActorAdminId(this HttpContext http)
    {
        var claim = http.User.FindFirst(ClaimTypes.NameIdentifier);
        if (claim is null || !Guid.TryParse(claim.Value, out var id))
            throw new InvalidOperationException("Admin context not found");
        return id;
    }
}
