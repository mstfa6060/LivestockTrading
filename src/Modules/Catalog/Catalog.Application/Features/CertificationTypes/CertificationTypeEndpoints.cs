using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace LivestockTrading.Catalog.Application.Features.CertificationTypes;

/// <summary>
/// CertificationType endpoint registration — MapGroup wiring for 3 CertificationType admin endpoints.
/// Route group: /admin/catalog/certification-types (RequireRole admin/moderator, tag Admin: Catalog).
/// Host application invokes MapCertificationTypeEndpoints in startup configuration.
/// Deactivate POST /{id:int}/deactivate — doc 06-api §8:812 DELETE → port DeactivateCertificationTypeAsync
/// baskin (KAYDET-14; IReferenceDataRepository Remove yok, FK guard); doc-port inconsistency Wave-2-sonu retro.
/// Auth enforcement (RequireRole) host-auth wave'inde aktif olur; W2.5 endpoint'leri
/// yapisal tam, JWT bearer henuz host'ta yapilandirilmadi (auth-inert kabul).
/// </summary>
public static class CertificationTypeEndpoints
{
    public static IEndpointRouteBuilder MapCertificationTypeEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/admin/catalog/certification-types")
            .RequireAuthorization(policy => policy.RequireRole("admin", "moderator"))
            .WithTags("Admin: Catalog");

        group.MapPost("/", CreateCertificationTypeEndpoint.Handle);
        group.MapPatch("/{id:int}", UpdateCertificationTypeEndpoint.Handle);
        group.MapPost("/{id:int}/deactivate", DeactivateCertificationTypeEndpoint.Handle);

        return app;
    }
}
