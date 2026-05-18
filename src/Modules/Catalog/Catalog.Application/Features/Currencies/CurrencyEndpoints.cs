using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace LivestockTrading.Catalog.Application.Features.Currencies;

/// <summary>
/// Currency admin endpoint registration — MapGroup wiring for 2 Currency admin endpoints.
/// Route group: /admin/catalog/currencies (RequireRole admin/moderator, tag Admin: Catalog).
/// W2.4-C: host-inert (S4, Program.cs wire Wave 3'te). MapCurrencyEndpoints çağrılmıyor.
/// Test handler-direct (W2.4-D scope). RefreshExchangeRates handler Wave 3 NotImpl stub.
/// </summary>
public static class CurrencyEndpoints
{
    public static IEndpointRouteBuilder MapCurrencyEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/admin/catalog/currencies")
            .RequireAuthorization(policy => policy.RequireRole("admin", "moderator"))
            .WithTags("Admin: Catalog");

        group.MapPost("/{id:int}/toggle-active", ToggleCurrencyActiveEndpoint.Handle);
        group.MapPost("/refresh-rates", RefreshExchangeRatesEndpoint.Handle);

        return app;
    }
}
