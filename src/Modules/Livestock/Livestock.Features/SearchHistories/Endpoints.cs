using FastEndpoints;
using Livestock.Domain.Entities;
using Livestock.Persistence;
using Microsoft.EntityFrameworkCore;
using Shared.Abstractions.Identity;

namespace Livestock.Features.SearchHistories;

public class GetSearchHistoryEndpoint(LivestockDbContext db, IUserContext user) : Endpoint<GetSearchHistoryRequest, List<SearchHistoryItem>>
{
    public override void Configure()
    {
        Post("/livestocktrading/SearchHistories/All");
        Tags("SearchHistories");
    }

    public override async Task HandleAsync(GetSearchHistoryRequest req, CancellationToken ct)
    {
        var pageSize = Math.Min(req.PageSize, 100);
        var skip = (req.Page - 1) * pageSize;

        var items = await db.SearchHistories.AsNoTracking()
            .Where(h => h.UserId == user.UserId && !h.IsDeleted)
            .OrderByDescending(h => h.SearchedAt)
            .Skip(skip).Take(pageSize)
            .Select(h => new SearchHistoryItem(h.Id, h.SearchQuery, h.Filters, h.ResultsCount, h.SearchedAt))
            .ToListAsync(ct);

        await SendAsync(items, 200, ct);
    }
}

public class RecordSearchEndpoint(LivestockDbContext db, IUserContext user) : Endpoint<RecordSearchRequest, EmptyResponse>
{
    public override void Configure()
    {
        Post("/livestocktrading/SearchHistories/Record");
        Tags("SearchHistories");
    }

    public override async Task HandleAsync(RecordSearchRequest req, CancellationToken ct)
    {
        var h = new SearchHistory
        {
            UserId = user.UserId,
            SearchQuery = req.SearchQuery,
            Filters = req.Filters,
            ResultsCount = req.ResultsCount,
            SearchedAt = DateTime.UtcNow
        };
        db.SearchHistories.Add(h);
        await db.SaveChangesAsync(ct);
        await SendNoContentAsync(ct);
    }
}

public class ClearSearchHistoryEndpoint(LivestockDbContext db, IUserContext user) : EndpointWithoutRequest
{
    public override void Configure()
    {
        Post("/livestocktrading/SearchHistories/Clear");
        Tags("SearchHistories");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var records = await db.SearchHistories
            .Where(h => h.UserId == user.UserId && !h.IsDeleted)
            .ToListAsync(ct);

        foreach (var r in records)
        { r.IsDeleted = true; r.DeletedAt = DateTime.UtcNow; }

        await db.SaveChangesAsync(ct);
        await SendNoContentAsync(ct);
    }
}
