using Livestock.Domain.Enums;

namespace Livestock.Features.Banners;

public record BannerItem(
    Guid Id,
    string Title,
    string? Subtitle,
    string ImageUrl,
    string? LinkUrl,
    BannerPosition Position,
    int SortOrder,
    bool IsActive,
    DateTime? StartsAt,
    DateTime? EndsAt,
    string? CountryCode,
    DateTime CreatedAt);

public record CreateBannerRequest(
    string Title,
    string? Subtitle,
    string ImageUrl,
    string? LinkUrl,
    BannerPosition Position,
    int SortOrder,
    bool IsActive,
    DateTime? StartsAt,
    DateTime? EndsAt,
    string? CountryCode);

public record UpdateBannerRequest(
    Guid Id,
    string Title,
    string? Subtitle,
    string ImageUrl,
    string? LinkUrl,
    BannerPosition Position,
    int SortOrder,
    bool IsActive,
    DateTime? StartsAt,
    DateTime? EndsAt,
    string? CountryCode);

public record GetBannersRequest(string? CountryCode = null);
public record GetBannerRequest(Guid Id);
public record DeleteBannerRequest(Guid Id);
