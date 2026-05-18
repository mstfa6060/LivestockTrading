using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace LivestockTrading.Catalog.Application.Features.Languages;

/// <summary>
/// Language admin endpoint registration — MapGroup wiring for 1 Language admin endpoint.
/// Route group: /admin/catalog/languages (RequireRole admin/moderator, tag Admin: Catalog).
/// W2.4-C: host-inert (S4, Program.cs wire Wave 3'te). MapLanguageEndpoints çağrılmıyor.
/// Test handler-direct (W2.4-D scope). Auth enforcement host-auth wave'inde aktif.
/// </summary>
public static class LanguageEndpoints
{
    public static IEndpointRouteBuilder MapLanguageEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/admin/catalog/languages")
            .RequireAuthorization(policy => policy.RequireRole("admin", "moderator"))
            .WithTags("Admin: Catalog");

        group.MapPost("/{id:int}/toggle-active", ToggleLanguageActiveEndpoint.Handle);

        return app;
    }
}
