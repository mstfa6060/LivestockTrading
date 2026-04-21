using System.Security.Claims;
using FastEndpoints;
using Files.Domain.Errors;
using Files.Features.Services;
using Files.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Files.Features.Buckets.Delete;

public sealed class DeleteBucketEndpoint(
    FilesDbContext db,
    IStorageService storage) : Endpoint<DeleteBucketRequest, DeleteBucketResponse>
{
    public override void Configure()
    {
        Delete("/Buckets/{BucketId}");
        Tags("Buckets");
    }

    public override async Task HandleAsync(DeleteBucketRequest req, CancellationToken ct)
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

        // Delete files from storage
        foreach (var file in bucket.Files)
        {
            try { await storage.DeleteAsync(file.ObjectKey, ct); }
            catch { /* best-effort */ }
        }

        bucket.IsDeleted = true;
        bucket.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync(ct);

        await SendAsync(new DeleteBucketResponse(true), 200, ct);
    }
}
