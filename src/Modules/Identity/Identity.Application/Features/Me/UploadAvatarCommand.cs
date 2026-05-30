namespace LivestockTrading.Identity.Application.Features.Me;

/// <summary>
/// POST /identity/users/me/avatar — sets the user's avatar URL after the
/// endpoint has streamed the multipart payload to IFileStorage. The command
/// carries only the resolved URL because IFormFile / Stream are not safe to
/// serialise through MassTransit (B-W4.2-D-3 reconcile).
/// </summary>
public sealed record UploadAvatarCommand(Guid UserId, string AvatarUrl);
