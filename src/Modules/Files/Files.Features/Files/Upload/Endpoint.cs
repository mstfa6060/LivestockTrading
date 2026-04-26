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
    IStorageService storage,
    IImageProcessingService imageProcessing) : Endpoint<UploadRequest, UploadResponse>
{
    private const long MaxFileSizeBytes = 75 * 1024 * 1024; // 75 MB
    private const int ThumbnailMaxSize = 300;

    public override void Configure()
    {
        Post("/fileprovider/Files/Upload");
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

        MediaBucket bucket;
        if (req.BucketId == Guid.Empty)
        {
            // First upload on a fresh form — validator guarantees ModuleName
            // and BucketType are present. Spin up a bucket inline so the
            // frontend doesn't have to make a separate Buckets/Create round
            // trip before every listing-create flow.
            bucket = new MediaBucket
            {
                OwnerId = userId,
                Module = req.ModuleName!,
                EntityId = req.EntityId,
                BucketType = req.BucketType!.Value,
            };
            db.Buckets.Add(bucket);
            await db.SaveChangesAsync(ct);
            bucket.Files = new List<FileRecord>();
        }
        else
        {
            var existing = await db.Buckets
                .Include(b => b.Files)
                .FirstOrDefaultAsync(b => b.Id == req.BucketId, ct);

            if (existing is null)
            {
                AddError(FilesErrors.Buckets.NotFound);
                await SendErrorsAsync(404, ct);
                return;
            }

            if (existing.OwnerId != userId)
            {
                AddError(FilesErrors.Buckets.NotOwned);
                await SendErrorsAsync(403, ct);
                return;
            }

            bucket = existing;
        }

        var activeFils = bucket.Files.Where(f => !f.IsDeleted).ToList();

        if (bucket.BucketType == BucketType.Single && activeFils.Count >= 1)
        {
            AddError(FilesErrors.Files.SingleBucketFull);
            await SendErrorsAsync(400, ct);
            return;
        }

        await storage.EnsureBucketExistsAsync(ct);

        var fileId = Guid.NewGuid();
        var isImage = imageProcessing.IsImageContentType(formFile.ContentType);

        string objectKey;
        string contentType;
        string extension;
        long sizeBytes;
        int? width = null;
        int? height = null;
        string? thumbnailObjectKey = null;

        if (isImage)
        {
            using var sourceMem = new MemoryStream();
            using (var src = formFile.OpenReadStream())
            {
                await src.CopyToAsync(sourceMem, ct);
            }
            var sourceBytes = sourceMem.ToArray();

            var encoded = await imageProcessing.EncodeWebpAsync(sourceBytes, ct);
            extension = encoded.Extension;
            contentType = encoded.ContentType;
            sizeBytes = encoded.Bytes.LongLength;
            width = encoded.Width;
            height = encoded.Height;
            objectKey = $"{bucket.Module}/{bucket.Id}/{fileId}.{extension}";

            using (var encodedStream = new MemoryStream(encoded.Bytes))
            {
                await storage.UploadAsync(encodedStream, objectKey, contentType, sizeBytes, ct);
            }

            var thumbnail = await imageProcessing.CreateThumbnailAsync(sourceBytes, ThumbnailMaxSize, ct);
            thumbnailObjectKey = $"{bucket.Module}/{bucket.Id}/{fileId}_thumb.{thumbnail.Extension}";
            using var thumbStream = new MemoryStream(thumbnail.Bytes);
            await storage.UploadAsync(thumbStream, thumbnailObjectKey, thumbnail.ContentType, thumbnail.Bytes.LongLength, ct);
        }
        else
        {
            extension = Path.GetExtension(formFile.FileName).TrimStart('.').ToLowerInvariant();
            contentType = formFile.ContentType;
            sizeBytes = formFile.Length;
            objectKey = $"{bucket.Module}/{bucket.Id}/{fileId}.{extension}";

            using var stream = formFile.OpenReadStream();
            await storage.UploadAsync(stream, objectKey, contentType, sizeBytes, ct);
        }

        var isFirst = activeFils.Count == 0;
        var sortOrder = activeFils.Count > 0 ? activeFils.Max(f => f.SortOrder) + 1 : 0;

        var record = new FileRecord
        {
            Id = fileId,
            BucketId = bucket.Id,
            ObjectKey = objectKey,
            ThumbnailObjectKey = thumbnailObjectKey,
            OriginalName = formFile.FileName,
            ContentType = contentType,
            Extension = extension,
            SizeBytes = sizeBytes,
            IsCover = isFirst,
            SortOrder = sortOrder,
            Width = width,
            Height = height,
            IsImage = isImage,
            IsProcessed = isImage,
        };

        db.Files.Add(record);
        await db.SaveChangesAsync(ct);

        await SendAsync(new UploadResponse(
            record.Id,
            record.BucketId,
            record.ObjectKey,
            record.ThumbnailObjectKey,
            record.OriginalName,
            record.ContentType,
            record.Extension,
            record.SizeBytes,
            record.IsCover,
            record.SortOrder,
            record.Width,
            record.Height,
            record.IsImage,
            record.CreatedAt
        ), 201, ct);
    }
}
