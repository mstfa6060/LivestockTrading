using LivestockTrading.Identity.Application.Common;
using MassTransit;
using MassTransit.Mediator;
using Microsoft.AspNetCore.Http;
using Shared.Contracts.Storage;
using Shared.Results;

namespace LivestockTrading.Identity.Application.Features.Me;

public static class UploadAvatarEndpoint
{
    private const long MaxBytes = 5 * 1024 * 1024;
    private static readonly string[] AllowedContentTypes =
        ["image/jpeg", "image/png", "image/webp"];

    public static async Task<IResult> Handle(
        IFormFile file,
        IScopedMediator mediator,
        IFileStorage storage,
        HttpContext http,
        CancellationToken ct)
    {
        var userId = http.GetUserId();

        // Content-type whitelist (B-W4.2-D-4). Fail through Result.Failure +
        // ToApiResult so the error payload format stays consistent with the
        // mediator path (single MapError source).
        if (!AllowedContentTypes.Contains(file.ContentType))
            return Result.Failure(new Error(
                "INVALID_CONTENT_TYPE",
                "Yalniz jpeg, png ve webp izinli.")).ToApiResult();

        if (file.Length > MaxBytes)
            return Result.Failure(new Error(
                "INVALID_FILE_SIZE",
                "Maksimum dosya boyutu 5 MB.")).ToApiResult();

        var extension = file.ContentType switch
        {
            "image/jpeg" => "jpg",
            "image/png" => "png",
            "image/webp" => "webp",
            _ => "bin",
        };
        var key = $"{userId}/{Guid.CreateVersion7()}.{extension}";

        string url;
        await using (var stream = file.OpenReadStream())
        {
            url = await storage.UploadAsync("avatars", key, stream, file.ContentType, ct);
        }

        var client = mediator.CreateRequestClient<UploadAvatarCommand>();
        var response = await client.GetResponse<Result>(
            new UploadAvatarCommand(userId, url), ct);

        return response.Message.IsSuccess
            ? Results.Ok(new { avatarUrl = url })
            : response.Message.ToApiResult();
    }
}
