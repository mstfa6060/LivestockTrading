using FastEndpoints;
using Livestock.Domain.Entities;
using Livestock.Domain.Errors;
using Livestock.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Livestock.Features.Brands;

public class GetAllBrandsEndpoint(LivestockDbContext db) : EndpointWithoutRequest<List<BrandListItem>>
{
    public override void Configure()
    {
        Post("/livestocktrading/Brands/All");
        AllowAnonymous();
        Tags("Brands");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var brands = await db.Brands
            .AsNoTracking()
            .OrderBy(b => b.SortOrder).ThenBy(b => b.Name)
            .Select(b => new BrandListItem(b.Id, b.Name, b.Slug, b.Description, b.LogoUrl, b.IsActive, b.SortOrder, b.CreatedAt))
            .ToListAsync(ct);

        await SendAsync(brands, 200, ct);
    }
}

public class GetBrandEndpoint(LivestockDbContext db) : Endpoint<GetBrandRequest, BrandDetail>
{
    public override void Configure()
    {
        Post("/livestocktrading/Brands/Detail");
        AllowAnonymous();
        Tags("Brands");
    }

    public override async Task HandleAsync(GetBrandRequest req, CancellationToken ct)
    {
        var b = await db.Brands.AsNoTracking().FirstOrDefaultAsync(x => x.Id == req.Id, ct);
        if (b is null)
        {
            AddError(LivestockErrors.BrandErrors.BrandNotFound);
            await SendErrorsAsync(404, ct);
            return;
        }

        await SendAsync(new BrandDetail(b.Id, b.Name, b.Slug, b.Description, b.LogoUrl, b.WebsiteUrl, b.IsActive, b.SortOrder, b.CreatedAt), 200, ct);
    }
}

public class CreateBrandEndpoint(LivestockDbContext db) : Endpoint<CreateBrandRequest, BrandDetail>
{
    public override void Configure()
    {
        Post("/livestocktrading/Brands/Create");
        Roles("LivestockTrading.Admin", "LivestockTrading.Moderator");
        Tags("Brands");
    }

    public override async Task HandleAsync(CreateBrandRequest req, CancellationToken ct)
    {
        var slugExists = await db.Brands.AnyAsync(x => x.Slug == req.Slug, ct);
        if (slugExists)
        {
            AddError(x => x.Slug, LivestockErrors.BrandErrors.BrandSlugAlreadyExists);
            await SendErrorsAsync(409, ct);
            return;
        }

        var brand = new Brand
        {
            Name = req.Name, Slug = req.Slug, Description = req.Description,
            LogoUrl = req.LogoUrl, WebsiteUrl = req.WebsiteUrl,
            SortOrder = req.SortOrder, IsActive = req.IsActive
        };
        db.Brands.Add(brand);
        await db.SaveChangesAsync(ct);

        await SendAsync(new BrandDetail(brand.Id, brand.Name, brand.Slug, brand.Description, brand.LogoUrl, brand.WebsiteUrl, brand.IsActive, brand.SortOrder, brand.CreatedAt), 201, ct);
    }
}

public class UpdateBrandEndpoint(LivestockDbContext db) : Endpoint<UpdateBrandRequest, BrandDetail>
{
    public override void Configure()
    {
        Post("/livestocktrading/Brands/Update");
        Roles("LivestockTrading.Admin", "LivestockTrading.Moderator");
        Tags("Brands");
    }

    public override async Task HandleAsync(UpdateBrandRequest req, CancellationToken ct)
    {
        var brand = await db.Brands.FirstOrDefaultAsync(x => x.Id == req.Id, ct);
        if (brand is null)
        {
            AddError(LivestockErrors.BrandErrors.BrandNotFound);
            await SendErrorsAsync(404, ct);
            return;
        }

        var slugExists = await db.Brands.AnyAsync(x => x.Slug == req.Slug && x.Id != req.Id, ct);
        if (slugExists)
        {
            AddError(x => x.Slug, LivestockErrors.BrandErrors.BrandSlugAlreadyExists);
            await SendErrorsAsync(409, ct);
            return;
        }

        brand.Name = req.Name; brand.Slug = req.Slug; brand.Description = req.Description;
        brand.LogoUrl = req.LogoUrl; brand.WebsiteUrl = req.WebsiteUrl;
        brand.SortOrder = req.SortOrder; brand.IsActive = req.IsActive;
        brand.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync(ct);

        await SendAsync(new BrandDetail(brand.Id, brand.Name, brand.Slug, brand.Description, brand.LogoUrl, brand.WebsiteUrl, brand.IsActive, brand.SortOrder, brand.CreatedAt), 200, ct);
    }
}

public class DeleteBrandEndpoint(LivestockDbContext db) : Endpoint<DeleteBrandRequest, EmptyResponse>
{
    public override void Configure()
    {
        Post("/livestocktrading/Brands/Delete");
        Roles("LivestockTrading.Admin");
        Tags("Brands");
    }

    public override async Task HandleAsync(DeleteBrandRequest req, CancellationToken ct)
    {
        var brand = await db.Brands.FirstOrDefaultAsync(x => x.Id == req.Id, ct);
        if (brand is null)
        {
            AddError(LivestockErrors.BrandErrors.BrandNotFound);
            await SendErrorsAsync(404, ct);
            return;
        }

        brand.IsDeleted = true; brand.DeletedAt = DateTime.UtcNow;
        await db.SaveChangesAsync(ct);
        await SendNoContentAsync(ct);
    }
}
