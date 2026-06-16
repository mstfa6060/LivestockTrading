using FastEndpoints;
using Iam.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Iam.Features.Geography;

public sealed record GeoIpDetectCountryResponse(
    string CountryCode,
    int? CountryId,
    string CountryName
);

// Stub implementation: returns Turkey as the default country. The legacy
// GeoIpService that looked up MaxMind GeoLite2 was part of the removed
// ArfBlocks code; for now the frontend only uses this to prefill the
// register country picker, so the fallback is acceptable.
public sealed class GeoIpDetectCountryEndpoint(IamDbContext db)
    : EndpointWithoutRequest<GeoIpDetectCountryResponse>
{
    public override void Configure()
    {
        Post("/iam/GeoIp/DetectCountry");
        AllowAnonymous();
        Tags("Geography");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var tr = await db.Countries
            .AsNoTracking()
            .Where(c => c.Code == "TR")
            .Select(c => new { c.Id, c.Name })
            .FirstOrDefaultAsync(ct);

        await SendAsync(new GeoIpDetectCountryResponse(
            "TR",
            tr?.Id,
            tr?.Name ?? "Türkiye"
        ), 200, ct);
    }
}
