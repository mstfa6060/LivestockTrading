using FastEndpoints;
using Livestock.Domain.Entities;
using Livestock.Domain.Errors;
using Livestock.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Livestock.Features.Categories.Create;

public class CreateCategoryEndpoint(LivestockDbContext db) : Endpoint<CreateCategoryRequest, CategoryDetail>
{
    public override void Configure()
    {
        Post("/livestocktrading/Categories/Create");
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
