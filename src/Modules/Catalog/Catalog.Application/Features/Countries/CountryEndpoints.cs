using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace LivestockTrading.Catalog.Application.Features.Countries;

/// <summary>
/// Country admin endpoint registration — MapGroup wiring for 1 Country admin endpoint.
/// Route group: /admin/catalog/countries (RequireRole admin/moderator, tag Admin: Catalog).
/// W2.4-C: host-inert (S4, Program.cs wire Wave 3'te). MapCountryEndpoints çağrılmıyor.
/// Test handler-direct (W2.4-D scope). Auth enforcement host-auth wave'inde aktif.
/// </summary>
public static class CountryEndpoints
{
    public static IEndpointRouteBuilder MapCountryEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/admin/catalog/countries")
            .RequireAuthorization(policy => policy.RequireRole("admin", "moderator"))
            .WithTags("Admin: Catalog");

        group.MapPost("/{id:int}/toggle-active", ToggleCountryActiveEndpoint.Handle);

        return app;
    }
}
