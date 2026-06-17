using FastEndpoints;
using Livestock.Domain.Errors;
using Livestock.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Livestock.Features.Categories.Detail;

public class GetCategoryEndpoint(LivestockDbContext db) : Endpoint<GetCategoryRequest, CategoryDetail>
{
    public override void Configure()
    {
        Post("/livestocktrading/Categories/Detail");
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
