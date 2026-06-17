using FastEndpoints;
using Files.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Files.Features.Buckets.Detail;

// Legacy-compat list-files-in-bucket lookup. The frontend calls this from
// product list/detail pages to resolve mediaBucketId → file array, then
// builds image URLs from {path, variants}. Response fields (including the
// misspelled "extention") match the generated client.
public sealed record BucketDetailRequest(Guid BucketId, Guid? ChangeId);

public sealed record BucketFile(
    Guid Id,
    string Extention,
    string Name,
    string Path,
    string SecurePath,
    string ContentType,
    bool IsDefault,
    int Index
);

public sealed record BucketDetailResponse(
    Guid BucketId,
    Guid ChangeId,
    List<BucketFile> Files
);

public sealed class BucketDetailEndpoint(FilesDbContext db)
    : Endpoint<BucketDetailRequest, BucketDetailResponse>
{
    public override void Configure()
    {
        Post("/fileprovider/Buckets/Detail");
        AllowAnonymous();
        Tags("Buckets");
    }

    public override async Task HandleAsync(BucketDetailRequest req, CancellationToken ct)
    {
        var files = await db.Files
            .AsNoTracking()
            .Where(f => f.BucketId == req.BucketId && !f.IsDeleted)
            .OrderBy(f => f.SortOrder)
            .Select(f => new BucketFile(
                f.Id,
                f.Extension,
                f.OriginalName,
                f.ObjectKey,
                f.ObjectKey,
                f.ContentType,
                f.IsCover,
                f.SortOrder))
            .ToListAsync(ct);

        await SendAsync(new BucketDetailResponse(
            req.BucketId,
            req.ChangeId ?? Guid.Empty,
            files), 200, ct);
    }
}
