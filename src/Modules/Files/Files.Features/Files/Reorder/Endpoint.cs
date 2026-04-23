using System.Security.Claims;
using FastEndpoints;
using Files.Domain.Errors;
using Files.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Files.Features.Files.Reorder;

public sealed class ReorderEndpoint(FilesDbContext db) : Endpoint<ReorderRequest, ReorderResponse>
{
    public override void Configure()
    {
        Post("/Files/Reorder");
        Tags("Files");
    }

    public override async Task HandleAsync(ReorderRequest req, CancellationToken ct)
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userIdStr, out var userId))
        {
            await SendUnauthorizedAsync(ct);
            return;
        }

        var bucket = await db.Buckets
            .Include(b => b.Files)
            .FirstOrDefaultAsync(b => b.Id == req.BucketId, ct);

        if (bucket is null)
        {
            AddError(FilesErrors.Buckets.NotFound);
            await SendErrorsAsync(404, ct);
            return;
        }

        if (bucket.OwnerId != userId)
        {
            AddError(FilesErrors.Buckets.NotOwned);
            await SendErrorsAsync(403, ct);
            return;
        }

        var activeFiles = bucket.Files.Where(f => !f.IsDeleted).ToList();
        var activeIds = activeFiles.Select(f => f.Id).ToHashSet();

        if (!req.FileIds.All(id => activeIds.Contains(id)) || req.FileIds.Count != activeIds.Count)
        {
            AddError(FilesErrors.Files.OrderInvalid);
            await SendErrorsAsync(400, ct);
            return;
        }

        for (var i = 0; i < req.FileIds.Count; i++)
        {
            var file = activeFiles.First(f => f.Id == req.FileIds[i]);
            file.SortOrder = i;
            file.UpdatedAt = DateTime.UtcNow;
        }

        await db.SaveChangesAsync(ct);

        await SendAsync(new ReorderResponse(true), 200, ct);
    }
}
