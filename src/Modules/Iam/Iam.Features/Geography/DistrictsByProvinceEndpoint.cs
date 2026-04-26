using FastEndpoints;
using Iam.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Iam.Features.Geography;

public sealed record DistrictsByProvinceRequest(int ProvinceId, string? Keyword);

public sealed record DistrictItem(
    int Id,
    string Name,
    int ProvinceId,
    string? NameTranslations,
    double? Latitude,
    double? Longitude,
    long? Population,
    string? Timezone
);

public sealed class DistrictsByProvinceEndpoint(IamDbContext db)
    : Endpoint<DistrictsByProvinceRequest, List<DistrictItem>>
{
    public override void Configure()
    {
        Post("/iam/Districts/ByProvince");
        AllowAnonymous();
        Tags("Geography");
    }

    public override async Task HandleAsync(DistrictsByProvinceRequest req, CancellationToken ct)
    {
        if (req.ProvinceId <= 0)
        {
            await SendAsync([], 200, ct);
            return;
        }

        var query = db.Districts
            .AsNoTracking()
            .Where(d => d.ProvinceId == req.ProvinceId);

        if (!string.IsNullOrWhiteSpace(req.Keyword))
        {
            var k = req.Keyword.Trim().ToLower();
            query = query.Where(d => d.Name.ToLower().Contains(k));
        }

        var list = await query
            .OrderBy(d => d.SortOrder).ThenBy(d => d.Name)
            .Select(d => new DistrictItem(
                d.Id,
                d.Name,
                d.ProvinceId,
                d.NameTranslations,
                d.Latitude,
                d.Longitude,
                d.Population,
                d.Timezone))
            .ToListAsync(ct);

        await SendAsync(list, 200, ct);
    }
}
