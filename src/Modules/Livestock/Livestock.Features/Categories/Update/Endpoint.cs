using FastEndpoints;
using Livestock.Domain.Errors;
using Livestock.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Livestock.Features.Categories.Update;

public class UpdateCategoryEndpoint(LivestockDbContext db) : Endpoint<UpdateCategoryRequest, CategoryDetail>
{
    public override void Configure()
    {
        Post("/livestocktrading/Categories/Update");
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
