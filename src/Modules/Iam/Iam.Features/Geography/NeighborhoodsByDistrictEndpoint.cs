using FastEndpoints;
using Iam.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Iam.Features.Geography;

public sealed record NeighborhoodsByDistrictRequest(int DistrictId, string? Keyword);

public sealed record NeighborhoodItem(
    int Id,
    string Name,
    int DistrictId,
    string? PostalCode,
    double? Latitude,
    double? Longitude
);

public sealed class NeighborhoodsByDistrictEndpoint(IamDbContext db)
    : Endpoint<NeighborhoodsByDistrictRequest, List<NeighborhoodItem>>
{
    public override void Configure()
    {
        Post("/iam/Neighborhoods/ByDistrict");
        AllowAnonymous();
        Tags("Geography");
    }

    public override async Task HandleAsync(NeighborhoodsByDistrictRequest req, CancellationToken ct)
    {
        if (req.DistrictId <= 0)
        {
            await SendAsync([], 200, ct);
            return;
        }

        var query = db.Neighborhoods
            .AsNoTracking()
            .Where(n => n.DistrictId == req.DistrictId);

        if (!string.IsNullOrWhiteSpace(req.Keyword))
        {
            var k = req.Keyword.Trim().ToLower();
            query = query.Where(n => n.Name.ToLower().Contains(k));
        }

        var list = await query
            .OrderBy(n => n.SortOrder).ThenBy(n => n.Name)
            .Select(n => new NeighborhoodItem(
                n.Id,
                n.Name,
                n.DistrictId,
                n.PostalCode,
                n.Latitude,
                n.Longitude))
            .ToListAsync(ct);

        await SendAsync(list, 200, ct);
    }
}
