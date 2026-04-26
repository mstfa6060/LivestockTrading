using FastEndpoints;
using Files.Domain.Errors;
using Files.Features.Services;
using Files.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Files.Features.Files.GetPresignedUrl;

public sealed class GetPresignedUrlEndpoint(
    FilesDbContext db,
    IStorageService storage) : Endpoint<GetPresignedUrlRequest, GetPresignedUrlResponse>
{
    public override void Configure()
    {
        Post("/fileprovider/Files/PresignedUrl");
        Tags("Files");
    }

    public override async Task HandleAsync(GetPresignedUrlRequest req, CancellationToken ct)
    {
        var file = await db.Files
            .AsNoTracking()
            .FirstOrDefaultAsync(f => f.Id == req.FileId, ct);

        if (file is null)
        {
            AddError(FilesErrors.Files.NotFound);
            await SendErrorsAsync(404, ct);
            return;
        }

        var url = await storage.GetPresignedUrlAsync(file.ObjectKey, req.ExpirySeconds, ct);
        var expiresAt = DateTime.UtcNow.AddSeconds(req.ExpirySeconds);

        await SendAsync(new GetPresignedUrlResponse(file.Id, url, expiresAt), 200, ct);
    }
}
