namespace Livestock.Features.Admin.AppVersions;

public record AppVersionItem(
    Guid Id,
    int Platform,
    string MinSupportedVersion,
    string LatestVersion,
    string StoreUrl,
    string? UpdateMessage,
    bool IsActive,
    DateTime CreatedAt,
    DateTime? UpdatedAt);

public record CreateAppVersionRequest(
    int Platform,
    string MinSupportedVersion,
    string LatestVersion,
    string StoreUrl,
    string? UpdateMessage);

public record UpdateAppVersionRequest(
    Guid Id,
    string MinSupportedVersion,
    string LatestVersion,
    string StoreUrl,
    string? UpdateMessage,
    bool IsActive);

public record GetAppVersionRequest(Guid Id);
public record DeleteAppVersionRequest(Guid Id);
public record CheckAppVersionRequest(int Platform);
public record AppVersionCheckResult(string MinSupportedVersion, string LatestVersion, string StoreUrl, string? UpdateMessage, bool IsActive);
