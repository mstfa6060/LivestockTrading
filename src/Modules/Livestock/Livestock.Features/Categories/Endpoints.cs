using FastEndpoints;
using Livestock.Domain.Entities;
using Livestock.Domain.Errors;
using Livestock.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Livestock.Features.Categories;

public class GetAllCategoriesEndpoint(LivestockDbContext db) : EndpointWithoutRequest<List<CategoryListItem>>
{
    public override void Configure()
    {
        Get("/Categories");
        AllowAnonymous();
        Tags("Categories");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var categories = await db.Categories
            .AsNoTracking()
            .Include(c => c.ParentCategory)
            .Include(c => c.SubCategories)
            .OrderBy(c => c.SortOrder).ThenBy(c => c.Name)
            .Select(c => new CategoryListItem(
                c.Id, c.Name, c.Slug, c.Description,
                c.IconUrl, c.ImageUrl, c.SortOrder, c.IsActive,
                c.ParentCategoryId, c.ParentCategory!.Name,
                c.SubCategories.Count(s => !s.IsDeleted),
                c.CreatedAt))
            .ToListAsync(ct);

        await SendAsync(categories, 200, ct);
    }
}

public class GetCategoryEndpoint(LivestockDbContext db) : Endpoint<GetCategoryRequest, CategoryDetail>
{
    public override void Configure()
    {
        Get("/Categories/{Id}");
        AllowAnonymous();
        Tags("Categories");
    }

    public override async Task HandleAsync(GetCategoryRequest req, CancellationToken ct)
    {
        var c = await db.Categories
            .AsNoTracking()
            .Include(x => x.ParentCategory)
            .FirstOrDefaultAsync(x => x.Id == req.Id, ct);

        if (c is null)
        {
            AddError(LivestockErrors.CategoryErrors.CategoryNotFound);
            await SendErrorsAsync(404, ct);
            return;
        }

        await SendAsync(new CategoryDetail(
            c.Id, c.Name, c.Slug, c.Description, c.IconUrl, c.ImageUrl,
            c.SortOrder, c.IsActive, c.ParentCategoryId, c.ParentCategory?.Name,
            c.NameTranslations, c.DescriptionTranslations, c.CreatedAt), 200, ct);
    }
}

public class CreateCategoryEndpoint(LivestockDbContext db) : Endpoint<CreateCategoryRequest, CategoryDetail>
{
    public override void Configure()
    {
        Post("/Categories");
        Roles("LivestockTrading.Admin", "LivestockTrading.Moderator");
        Tags("Categories");
    }

    public override async Task HandleAsync(CreateCategoryRequest req, CancellationToken ct)
    {
        var slugExists = await db.Categories.AnyAsync(x => x.Slug == req.Slug, ct);
        if (slugExists)
        {
            AddError(x => x.Slug, LivestockErrors.CategoryErrors.CategorySlugAlreadyExists);
            await SendErrorsAsync(409, ct);
            return;
        }

        var category = new Category
        {
            Name = req.Name,
            Slug = req.Slug,
            Description = req.Description,
            IconUrl = req.IconUrl,
            ImageUrl = req.ImageUrl,
            ParentCategoryId = req.ParentCategoryId,
            SortOrder = req.SortOrder,
            IsActive = req.IsActive
        };

        db.Categories.Add(category);
        await db.SaveChangesAsync(ct);

        await SendAsync(new CategoryDetail(
            category.Id, category.Name, category.Slug, category.Description,
            category.IconUrl, category.ImageUrl, category.SortOrder, category.IsActive,
            category.ParentCategoryId, null, null, null, category.CreatedAt), 201, ct);
    }
}

public class UpdateCategoryEndpoint(LivestockDbContext db) : Endpoint<UpdateCategoryRequest, CategoryDetail>
{
    public override void Configure()
    {
        Put("/Categories/{Id}");
        Roles("LivestockTrading.Admin", "LivestockTrading.Moderator");
        Tags("Categories");
    }

    public override async Task HandleAsync(UpdateCategoryRequest req, CancellationToken ct)
    {
        var category = await db.Categories.FirstOrDefaultAsync(x => x.Id == req.Id, ct);
        if (category is null)
        {
            AddError(LivestockErrors.CategoryErrors.CategoryNotFound);
            await SendErrorsAsync(404, ct);
            return;
        }

        var slugExists = await db.Categories.AnyAsync(x => x.Slug == req.Slug && x.Id != req.Id, ct);
        if (slugExists)
        {
            AddError(x => x.Slug, LivestockErrors.CategoryErrors.CategorySlugAlreadyExists);
            await SendErrorsAsync(409, ct);
            return;
        }

        category.Name = req.Name;
        category.Slug = req.Slug;
        category.Description = req.Description;
        category.IconUrl = req.IconUrl;
        category.ImageUrl = req.ImageUrl;
        category.ParentCategoryId = req.ParentCategoryId;
        category.SortOrder = req.SortOrder;
        category.IsActive = req.IsActive;
        category.UpdatedAt = DateTime.UtcNow;

        await db.SaveChangesAsync(ct);

        await SendAsync(new CategoryDetail(
            category.Id, category.Name, category.Slug, category.Description,
            category.IconUrl, category.ImageUrl, category.SortOrder, category.IsActive,
            category.ParentCategoryId, null, null, null, category.CreatedAt), 200, ct);
    }
}

public class DeleteCategoryEndpoint(LivestockDbContext db) : Endpoint<DeleteCategoryRequest, EmptyResponse>
{
    public override void Configure()
    {
        Delete("/Categories/{Id}");
        Roles("LivestockTrading.Admin");
        Tags("Categories");
    }

    public override async Task HandleAsync(DeleteCategoryRequest req, CancellationToken ct)
    {
        var category = await db.Categories.FirstOrDefaultAsync(x => x.Id == req.Id, ct);
        if (category is null)
        {
            AddError(LivestockErrors.CategoryErrors.CategoryNotFound);
            await SendErrorsAsync(404, ct);
            return;
        }

        category.IsDeleted = true;
        category.DeletedAt = DateTime.UtcNow;
        await db.SaveChangesAsync(ct);

        await SendNoContentAsync(ct);
    }
}
