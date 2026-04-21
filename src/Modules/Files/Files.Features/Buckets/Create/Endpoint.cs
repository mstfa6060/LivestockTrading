using System.Security.Claims;
using FastEndpoints;
using Files.Domain.Entities;
using Files.Persistence;

namespace Files.Features.Buckets.Create;

public sealed class CreateBucketEndpoint(FilesDbContext db) : Endpoint<CreateBucketRequest, CreateBucketResponse>
{
    public override void Configure()
    {
        Post("/Buckets/Create");
        Tags("Buckets");
    }

    public override async Task HandleAsync(CreateBucketRequest req, CancellationToken ct)
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userIdStr, out var userId))
        {
            await SendUnauthorizedAsync(ct);
            return;
        }

        var bucket = new MediaBucket
        {
            OwnerId = userId,
            Module = req.Module,
            EntityId = req.EntityId,
            BucketType = req.BucketType,
        };

        db.Buckets.Add(bucket);
        await db.SaveChangesAsync(ct);

        await SendAsync(new CreateBucketResponse(
            bucket.Id,
            bucket.Module,
            bucket.EntityId,
            bucket.BucketType,
            bucket.CreatedAt
        ), 201, ct);
    }
}
