using System.Security.Claims;
using FastEndpoints;
using Files.Domain.Errors;
using Files.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Files.Features.Files.SetCover;

public sealed class SetCoverEndpoint(FilesDbContext db) : Endpoint<SetCoverRequest, SetCoverResponse>
{
    public override void Configure()
    {
        Post("/Files/SetCover");
        Tags("Files");
    }

    public override async Task HandleAsync(SetCoverRequest req, CancellationToken ct)
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

        var targetFile = bucket.Files.FirstOrDefault(f => f.Id == req.FileId && !f.IsDeleted);
        if (targetFile is null)
        {
            AddError(FilesErrors.Files.NotFound);
            await SendErrorsAsync(404, ct);
            return;
        }

        foreach (var f in bucket.Files.Where(f => !f.IsDeleted))
        {
            f.IsCover = f.Id == req.FileId;
        }

        await db.SaveChangesAsync(ct);

        await SendAsync(new SetCoverResponse(true), 200, ct);
    }
}
