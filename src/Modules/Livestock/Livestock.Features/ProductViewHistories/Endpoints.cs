using FastEndpoints;
using Livestock.Domain.Entities;
using Livestock.Persistence;
using Microsoft.EntityFrameworkCore;
using Shared.Abstractions.Identity;

namespace Livestock.Features.ProductViewHistories;

public class GetProductViewHistoryEndpoint(LivestockDbContext db, IUserContext user) : Endpoint<GetProductViewHistoryRequest, List<ProductViewHistoryItem>>
{
    public override void Configure()
    {
        Get("/ProductViewHistories");
        Tags("ProductViewHistories");
    }

    public override async Task HandleAsync(GetProductViewHistoryRequest req, CancellationToken ct)
    {
        var pageSize = Math.Min(req.PageSize, 100);
        var skip = (req.Page - 1) * pageSize;

        var items = await db.ProductViewHistories.AsNoTracking()
            .Include(h => h.Product)
            .Where(h => h.UserId == user.UserId && !h.IsDeleted)
            .OrderByDescending(h => h.ViewedAt)
            .Skip(skip).Take(pageSize)
            .Select(h => new ProductViewHistoryItem(h.Id, h.ProductId, h.Product.Title, h.ViewedAt, h.ViewSource))
            .ToListAsync(ct);

        await SendAsync(items, 200, ct);
    }
}

public class RecordProductViewEndpoint(LivestockDbContext db, IUserContext user) : Endpoint<RecordProductViewRequest, EmptyResponse>
{
    public override void Configure()
    {
        Post("/ProductViewHistories");
        Tags("ProductViewHistories");
    }

    public override async Task HandleAsync(RecordProductViewRequest req, CancellationToken ct)
    {
        var h = new ProductViewHistory
        {
            UserId = user.UserId,
            ProductId = req.ProductId,
            ViewedAt = DateTime.UtcNow,
            ViewSource = req.ViewSource
        };
        db.ProductViewHistories.Add(h);
        await db.SaveChangesAsync(ct);
        await SendNoContentAsync(ct);
    }
}

public class ClearProductViewHistoryEndpoint(LivestockDbContext db, IUserContext user) : EndpointWithoutRequest
{
    public override void Configure()
    {
        Delete("/ProductViewHistories");
        Tags("ProductViewHistories");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var records = await db.ProductViewHistories
            .Where(h => h.UserId == user.UserId && !h.IsDeleted)
            .ToListAsync(ct);

        foreach (var r in records)
        { r.IsDeleted = true; r.DeletedAt = DateTime.UtcNow; }

        await db.SaveChangesAsync(ct);
        await SendNoContentAsync(ct);
    }
}
