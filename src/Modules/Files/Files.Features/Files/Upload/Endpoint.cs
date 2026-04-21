using System.Security.Claims;
using FastEndpoints;
using Files.Domain.Entities;
using Files.Domain.Enums;
using Files.Domain.Errors;
using Files.Features.Services;
using Files.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Files.Features.Files.Upload;

public sealed class UploadEndpoint(
    FilesDbContext db,
    IStorageService storage) : Endpoint<UploadRequest, UploadResponse>
{
    private const long MaxFileSizeBytes = 75 * 1024 * 1024; // 75 MB

    public override void Configure()
    {
        Post("/Files/Upload");
        AllowFileUploads();
        Tags("Files");
    }

    public override async Task HandleAsync(UploadRequest req, CancellationToken ct)
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userIdStr, out var userId))
        {
            await SendUnauthorizedAsync(ct);
            return;
        }

        if (Files.Count == 0)
        {
            AddError("Yüklenecek dosya bulunamadı.");
            await SendErrorsAsync(400, ct);
            return;
        }

        var formFile = Files[0];

        if (formFile.Length > MaxFileSizeBytes)
        {
            AddError(FilesErrors.Files.TooLarge);
            await SendErrorsAsync(400, ct);
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

        var activeFils = bucket.Files.Where(f => !f.IsDeleted).ToList();

        if (bucket.BucketType == BucketType.Single && activeFils.Count >= 1)
        {
            AddError(FilesErrors.Files.SingleBucketFull);
            await SendErrorsAsync(400, ct);
            return;
        }

        var extension = Path.GetExtension(formFile.FileName).TrimStart('.').ToLowerInvariant();
        var fileId = Guid.NewGuid();
        var objectKey = $"{bucket.Module}/{bucket.Id}/{fileId}.{extension}";

        await storage.EnsureBucketExistsAsync(ct);

        using var stream = formFile.OpenReadStream();
        await storage.UploadAsync(stream, objectKey, formFile.ContentType, formFile.Length, ct);

        var isFirst = activeFils.Count == 0;
        var sortOrder = activeFils.Count > 0 ? activeFils.Max(f => f.SortOrder) + 1 : 0;

        var record = new FileRecord
        {
            Id = fileId,
            BucketId = bucket.Id,
            ObjectKey = objectKey,
            OriginalName = formFile.FileName,
            ContentType = formFile.ContentType,
            Extension = extension,
            SizeBytes = formFile.Length,
            IsCover = isFirst,
            SortOrder = sortOrder,
        };

        db.Files.Add(record);
        await db.SaveChangesAsync(ct);

        await SendAsync(new UploadResponse(
            record.Id,
            record.BucketId,
            record.ObjectKey,
            record.OriginalName,
            record.ContentType,
            record.Extension,
            record.SizeBytes,
            record.IsCover,
            record.SortOrder,
            record.Width,
            record.Height,
            record.CreatedAt
        ), 201, ct);
    }
}
