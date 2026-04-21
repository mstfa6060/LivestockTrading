using FastEndpoints;
using Livestock.Domain.Entities;
using Livestock.Domain.Errors;
using Livestock.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Livestock.Features.Banners;

public class GetBannersEndpoint(LivestockDbContext db) : Endpoint<GetBannersRequest, List<BannerItem>>
{
    public override void Configure()
    {
        Get("/Banners");
        AllowAnonymous();
        Tags("Banners");
    }

    public override async Task HandleAsync(GetBannersRequest req, CancellationToken ct)
    {
        var now = DateTime.UtcNow;
        var query = db.Banners
            .AsNoTracking()
            .Where(b => b.IsActive && !b.IsDeleted
                && (b.StartsAt == null || b.StartsAt <= now)
                && (b.EndsAt == null || b.EndsAt >= now));

        if (!string.IsNullOrWhiteSpace(req.CountryCode))
        {
            query = query.Where(b => b.CountryCode == null || b.CountryCode == req.CountryCode);
        }

        var banners = await query
            .OrderBy(b => b.SortOrder)
            .Select(b => new BannerItem(b.Id, b.Title, b.Subtitle, b.ImageUrl, b.LinkUrl,
                b.Position, b.SortOrder, b.IsActive, b.StartsAt, b.EndsAt, b.CountryCode, b.CreatedAt))
            .ToListAsync(ct);

        await SendAsync(banners, 200, ct);
    }
}

public class GetBannerEndpoint(LivestockDbContext db) : Endpoint<GetBannerRequest, BannerItem>
{
    public override void Configure()
    {
        Get("/Banners/{Id}");
        Roles("LivestockTrading.Admin", "LivestockTrading.Moderator");
        Tags("Banners");
    }

    public override async Task HandleAsync(GetBannerRequest req, CancellationToken ct)
    {
        var b = await db.Banners.AsNoTracking().FirstOrDefaultAsync(x => x.Id == req.Id && !x.IsDeleted, ct);
        if (b is null)
        {
            AddError(LivestockErrors.Common.NotFound);
            await SendErrorsAsync(404, ct);
            return;
        }

        await SendAsync(new BannerItem(b.Id, b.Title, b.Subtitle, b.ImageUrl, b.LinkUrl,
            b.Position, b.SortOrder, b.IsActive, b.StartsAt, b.EndsAt, b.CountryCode, b.CreatedAt), 200, ct);
    }
}

public class CreateBannerEndpoint(LivestockDbContext db) : Endpoint<CreateBannerRequest, BannerItem>
{
    public override void Configure()
    {
        Post("/Banners");
        Roles("LivestockTrading.Admin", "LivestockTrading.Moderator");
        Tags("Banners");
    }

    public override async Task HandleAsync(CreateBannerRequest req, CancellationToken ct)
    {
        var banner = new Banner
        {
            Title = req.Title,
            Subtitle = req.Subtitle,
            ImageUrl = req.ImageUrl,
            LinkUrl = req.LinkUrl,
            Position = req.Position,
            SortOrder = req.SortOrder,
            IsActive = req.IsActive,
            StartsAt = req.StartsAt,
            EndsAt = req.EndsAt,
            CountryCode = req.CountryCode
        };
        db.Banners.Add(banner);
        await db.SaveChangesAsync(ct);

        await SendAsync(new BannerItem(banner.Id, banner.Title, banner.Subtitle, banner.ImageUrl, banner.LinkUrl,
            banner.Position, banner.SortOrder, banner.IsActive, banner.StartsAt, banner.EndsAt, banner.CountryCode, banner.CreatedAt), 201, ct);
    }
}

public class UpdateBannerEndpoint(LivestockDbContext db) : Endpoint<UpdateBannerRequest, BannerItem>
{
    public override void Configure()
    {
        Put("/Banners/{Id}");
        Roles("LivestockTrading.Admin", "LivestockTrading.Moderator");
        Tags("Banners");
    }

    public override async Task HandleAsync(UpdateBannerRequest req, CancellationToken ct)
    {
        var banner = await db.Banners.FirstOrDefaultAsync(b => b.Id == req.Id && !b.IsDeleted, ct);
        if (banner is null)
        {
            AddError(LivestockErrors.Common.NotFound);
            await SendErrorsAsync(404, ct);
            return;
        }

        banner.Title = req.Title;
        banner.Subtitle = req.Subtitle;
        banner.ImageUrl = req.ImageUrl;
        banner.LinkUrl = req.LinkUrl;
        banner.Position = req.Position;
        banner.SortOrder = req.SortOrder;
        banner.IsActive = req.IsActive;
        banner.StartsAt = req.StartsAt;
        banner.EndsAt = req.EndsAt;
        banner.CountryCode = req.CountryCode;
        banner.UpdatedAt = DateTime.UtcNow;

        await db.SaveChangesAsync(ct);

        await SendAsync(new BannerItem(banner.Id, banner.Title, banner.Subtitle, banner.ImageUrl, banner.LinkUrl,
            banner.Position, banner.SortOrder, banner.IsActive, banner.StartsAt, banner.EndsAt, banner.CountryCode, banner.CreatedAt), 200, ct);
    }
}

public class DeleteBannerEndpoint(LivestockDbContext db) : Endpoint<DeleteBannerRequest, EmptyResponse>
{
    public override void Configure()
    {
        Delete("/Banners/{Id}");
        Roles("LivestockTrading.Admin", "LivestockTrading.Moderator");
        Tags("Banners");
    }

    public override async Task HandleAsync(DeleteBannerRequest req, CancellationToken ct)
    {
        var banner = await db.Banners.FirstOrDefaultAsync(b => b.Id == req.Id && !b.IsDeleted, ct);
        if (banner is null)
        {
            AddError(LivestockErrors.Common.NotFound);
            await SendErrorsAsync(404, ct);
            return;
        }

        banner.IsDeleted = true;
        banner.DeletedAt = DateTime.UtcNow;
        await db.SaveChangesAsync(ct);

        await SendNoContentAsync(ct);
    }
}
