using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace LivestockTrading.Catalog.Application.Features.BorderRules;

/// <summary>
/// BorderRule endpoint registration — MapGroup wiring for 3 BorderRule admin endpoints.
/// Route group: /admin/catalog/border-rules (RequireRole admin/moderator, tag Admin: Catalog).
/// Host application invokes MapBorderRuleEndpoints in startup configuration.
/// {id:guid} route (BorderRule Guid v7 PK — CertType/Breed {id:int} ile asimetri).
/// GET /admin/catalog/border-rules?cursor= (doc §8:806) port'ta read yok → W2.6 AdminReadService scope (K5).
/// Auth enforcement (RequireRole) host-auth wave'inde aktif olur; W2.5 endpoint'leri
/// yapisal tam, JWT bearer henuz host'ta yapilandirilmadi (auth-inert kabul).
/// </summary>
public static class BorderRuleEndpoints
{
    public static IEndpointRouteBuilder MapBorderRuleEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/admin/catalog/border-rules")
            .RequireAuthorization(policy => policy.RequireRole("admin", "moderator"))
            .WithTags("Admin: Catalog");

        group.MapPost("/", CreateBorderRuleEndpoint.Handle);
        group.MapPatch("/{id:guid}", UpdateBorderRuleEndpoint.Handle);
        group.MapPost("/{id:guid}/deactivate", DeactivateBorderRuleEndpoint.Handle);

        return app;
    }
}
