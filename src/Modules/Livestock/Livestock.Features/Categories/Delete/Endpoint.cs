using FastEndpoints;
using Livestock.Domain.Errors;
using Livestock.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Livestock.Features.Categories.Delete;

public class DeleteCategoryEndpoint(LivestockDbContext db) : Endpoint<DeleteCategoryRequest, EmptyResponse>
{
    public override void Configure()
    {
        Post("/livestocktrading/Categories/Delete");
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
