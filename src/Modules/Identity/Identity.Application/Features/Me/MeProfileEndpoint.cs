using LivestockTrading.Identity.Application.Abstractions;
using Microsoft.AspNetCore.Http;
using LivestockTrading.Identity.Application.Common;

namespace LivestockTrading.Identity.Application.Features.Me;

/// <summary>
/// GET /identity/users/me — self-service profile read endpoint. Endpoint-direct
/// port consumption (no mediator pipeline) — read-only, no UoW or validator
/// needed. Catalog GetMissingTranslationsEndpoint pattern (B-W4.2-9 reconcile).
/// </summary>
public static class MeProfileEndpoint
{
    public static async Task<IResult> Handle(
        HttpContext http,
        IMeReadService readService,
        CancellationToken ct)
    {
        var userId = http.GetUserId();
        var profile = await readService.GetMyProfileAsync(userId, ct);
        return profile is null ? Results.NotFound() : Results.Ok(profile);
    }
}
