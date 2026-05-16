namespace Shared.Contracts.Catalog.Admin;

using Shared.ValueObjects;
using Shared.Contracts.Catalog;

/// <summary>Brand admin-list projection (CursorPage). Description/LogoUrl HARİÇ (list bandwidth); moderation için Status+suggester+CreatedAt.</summary>
public sealed record BrandListItem(
    Guid Id,
    string Slug,
    Translations Name,
    BrandStatus Status,
    bool IsActive,
    string? OriginCountryCode,
    Guid? SuggestedByUserId,
    DateTimeOffset? SuggestedAt,
    DateTimeOffset CreatedAt,
    int DisplayOrder);
