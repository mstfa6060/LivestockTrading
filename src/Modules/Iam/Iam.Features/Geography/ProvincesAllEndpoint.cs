using FastEndpoints;
using Iam.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Iam.Features.Geography;

public sealed record ProvincesAllRequest(int CountryId, string? Keyword);

public sealed record ProvinceItem(
    int Id,
    int CountryId,
    string Name,
    string? Code,
    string? NameTranslations,
    double? Latitude,
    double? Longitude,
    long? Population,
    string? Timezone
);

public sealed class ProvincesAllEndpoint(IamDbContext db) : Endpoint<ProvincesAllRequest, List<ProvinceItem>>
{
    public override void Configure()
    {
        Post("/iam/Provinces/All");
        AllowAnonymous();
        Tags("Geography");
    }

    public override async Task HandleAsync(ProvincesAllRequest req, CancellationToken ct)
    {
        if (req.CountryId <= 0)
        {
            await SendAsync([], 200, ct);
            return;
        }

        var query = db.Provinces
            .AsNoTracking()
            .Where(p => p.CountryId == req.CountryId);

        if (!string.IsNullOrWhiteSpace(req.Keyword))
        {
            var k = req.Keyword.Trim().ToLower();
            query = query.Where(p => p.Name.ToLower().Contains(k));
        }

        var list = await query
            .OrderBy(p => p.SortOrder).ThenBy(p => p.Name)
            .Select(p => new ProvinceItem(
                p.Id,
                p.CountryId,
                p.Name,
                p.Code,
                p.NameTranslations,
                p.Latitude,
                p.Longitude,
                p.Population,
                p.Timezone))
            .ToListAsync(ct);

        await SendAsync(list, 200, ct);
    }
}
