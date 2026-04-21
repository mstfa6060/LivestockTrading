namespace Livestock.Features.Brands;

public record BrandListItem(Guid Id, string Name, string Slug, string? Description, string? LogoUrl, bool IsActive, int SortOrder, DateTime CreatedAt);
public record BrandDetail(Guid Id, string Name, string Slug, string? Description, string? LogoUrl, string? WebsiteUrl, bool IsActive, int SortOrder, DateTime CreatedAt);
public record CreateBrandRequest(string Name, string Slug, string? Description, string? LogoUrl, string? WebsiteUrl, int SortOrder, bool IsActive);
public record UpdateBrandRequest(Guid Id, string Name, string Slug, string? Description, string? LogoUrl, string? WebsiteUrl, int SortOrder, bool IsActive);
public record DeleteBrandRequest(Guid Id);
public record GetBrandRequest(Guid Id);
