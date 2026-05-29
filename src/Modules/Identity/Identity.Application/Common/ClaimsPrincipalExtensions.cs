using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace LivestockTrading.Identity.Application.Common;

public static class ClaimsPrincipalExtensions
{
    /// <summary>
    /// Resolves authenticated user ID from HttpContext claims ("sub" / NameIdentifier).
    /// JWT "sub" claim is mapped to ClaimTypes.NameIdentifier by ASP.NET Core auth
    /// handler (default JwtBearer behavior).
    /// Throws InvalidOperationException when claim missing or unparseable — endpoints
    /// requiring authenticated context rely on auth middleware to short-circuit
    /// unauthenticated requests before reaching this helper.
    /// </summary>
    public static Guid GetUserId(this HttpContext http)
    {
        var claim = http.User.FindFirst(ClaimTypes.NameIdentifier);
        if (claim is null || !Guid.TryParse(claim.Value, out var id))
            throw new InvalidOperationException("User context not found");
        return id;
    }
}
