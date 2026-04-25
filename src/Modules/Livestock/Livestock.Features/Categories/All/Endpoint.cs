using FastEndpoints;
using Livestock.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Livestock.Features.Categories.All;

public class GetAllCategoriesEndpoint(LivestockDbContext db) : EndpointWithoutRequest<List<CategoryListItem>>
{
    public override void Configure()
    {
        Post("/livestocktrading/Categories/All");
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
