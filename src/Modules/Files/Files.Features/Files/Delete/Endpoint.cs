using System.Security.Claims;
using FastEndpoints;
using Files.Domain.Errors;
using Files.Features.Services;
using Files.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Files.Features.Files.Delete;

public sealed class DeleteFileEndpoint(
    FilesDbContext db,
    IStorageService storage) : Endpoint<DeleteFileRequest, DeleteFileResponse>
{
    public override void Configure()
    {
        Post("/fileprovider/Files/Delete");
        Tags("Files");
    }

    public override async Task HandleAsync(DeleteFileRequest req, CancellationToken ct)
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

        var file = bucket.Files.FirstOrDefault(f => f.Id == req.FileId);
        if (file is null)
        {
            AddError(FilesErrors.Files.NotFound);
            await SendErrorsAsync(404, ct);
            return;
        }

        try { await storage.DeleteAsync(file.ObjectKey, ct); }
        catch { /* best-effort: object may not exist */ }

        file.IsDeleted = true;
        file.UpdatedAt = DateTime.UtcNow;

        // If deleted file was cover, assign cover to next available
        if (file.IsCover)
        {
            var next = bucket.Files.FirstOrDefault(f => !f.IsDeleted && f.Id != file.Id);
            if (next is not null) { next.IsCover = true; }
        }

        await db.SaveChangesAsync(ct);

        await SendAsync(new DeleteFileResponse(true), 200, ct);
    }
}
