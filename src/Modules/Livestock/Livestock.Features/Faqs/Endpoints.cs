using FastEndpoints;
using Livestock.Domain.Entities;
using Livestock.Domain.Errors;
using Livestock.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Livestock.Features.Faqs;

public class GetFaqsEndpoint(LivestockDbContext db) : Endpoint<GetFaqsRequest, List<FaqItem>>
{
    public override void Configure()
    {
        Get("/Faqs");
        AllowAnonymous();
        Tags("Faqs");
    }

    public override async Task HandleAsync(GetFaqsRequest req, CancellationToken ct)
    {
        var query = db.Faqs.AsNoTracking().Where(f => f.IsActive && !f.IsDeleted);

        if (!string.IsNullOrWhiteSpace(req.LanguageCode))
        {
            query = query.Where(f => f.LanguageCode == null || f.LanguageCode == req.LanguageCode);
        }

        if (!string.IsNullOrWhiteSpace(req.Category))
        {
            query = query.Where(f => f.Category == req.Category);
        }

        var faqs = await query
            .OrderBy(f => f.SortOrder)
            .Select(f => new FaqItem(f.Id, f.Question, f.Answer, f.Category, f.SortOrder, f.IsActive, f.LanguageCode, f.CreatedAt))
            .ToListAsync(ct);

        await SendAsync(faqs, 200, ct);
    }
}

public class GetFaqEndpoint(LivestockDbContext db) : Endpoint<GetFaqRequest, FaqItem>
{
    public override void Configure()
    {
        Get("/Faqs/{Id}");
        Roles("LivestockTrading.Admin", "LivestockTrading.Moderator");
        Tags("Faqs");
    }

    public override async Task HandleAsync(GetFaqRequest req, CancellationToken ct)
    {
        var f = await db.Faqs.AsNoTracking().FirstOrDefaultAsync(x => x.Id == req.Id && !x.IsDeleted, ct);
        if (f is null)
        {
            AddError(LivestockErrors.Common.NotFound);
            await SendErrorsAsync(404, ct);
            return;
        }

        await SendAsync(new FaqItem(f.Id, f.Question, f.Answer, f.Category, f.SortOrder, f.IsActive, f.LanguageCode, f.CreatedAt), 200, ct);
    }
}

public class CreateFaqEndpoint(LivestockDbContext db) : Endpoint<CreateFaqRequest, FaqItem>
{
    public override void Configure()
    {
        Post("/Faqs");
        Roles("LivestockTrading.Admin", "LivestockTrading.Moderator");
        Tags("Faqs");
    }

    public override async Task HandleAsync(CreateFaqRequest req, CancellationToken ct)
    {
        var faq = new Faq
        {
            Question = req.Question,
            Answer = req.Answer,
            Category = req.Category,
            SortOrder = req.SortOrder,
            IsActive = req.IsActive,
            LanguageCode = req.LanguageCode
        };
        db.Faqs.Add(faq);
        await db.SaveChangesAsync(ct);

        await SendAsync(new FaqItem(faq.Id, faq.Question, faq.Answer, faq.Category, faq.SortOrder, faq.IsActive, faq.LanguageCode, faq.CreatedAt), 201, ct);
    }
}

public class UpdateFaqEndpoint(LivestockDbContext db) : Endpoint<UpdateFaqRequest, FaqItem>
{
    public override void Configure()
    {
        Put("/Faqs/{Id}");
        Roles("LivestockTrading.Admin", "LivestockTrading.Moderator");
        Tags("Faqs");
    }

    public override async Task HandleAsync(UpdateFaqRequest req, CancellationToken ct)
    {
        var faq = await db.Faqs.FirstOrDefaultAsync(f => f.Id == req.Id && !f.IsDeleted, ct);
        if (faq is null)
        {
            AddError(LivestockErrors.Common.NotFound);
            await SendErrorsAsync(404, ct);
            return;
        }

        faq.Question = req.Question;
        faq.Answer = req.Answer;
        faq.Category = req.Category;
        faq.SortOrder = req.SortOrder;
        faq.IsActive = req.IsActive;
        faq.LanguageCode = req.LanguageCode;
        faq.UpdatedAt = DateTime.UtcNow;

        await db.SaveChangesAsync(ct);

        await SendAsync(new FaqItem(faq.Id, faq.Question, faq.Answer, faq.Category, faq.SortOrder, faq.IsActive, faq.LanguageCode, faq.CreatedAt), 200, ct);
    }
}

public class DeleteFaqEndpoint(LivestockDbContext db) : Endpoint<DeleteFaqRequest, EmptyResponse>
{
    public override void Configure()
    {
        Delete("/Faqs/{Id}");
        Roles("LivestockTrading.Admin", "LivestockTrading.Moderator");
        Tags("Faqs");
    }

    public override async Task HandleAsync(DeleteFaqRequest req, CancellationToken ct)
    {
        var faq = await db.Faqs.FirstOrDefaultAsync(f => f.Id == req.Id && !f.IsDeleted, ct);
        if (faq is null)
        {
            AddError(LivestockErrors.Common.NotFound);
            await SendErrorsAsync(404, ct);
            return;
        }

        faq.IsDeleted = true;
        faq.DeletedAt = DateTime.UtcNow;
        await db.SaveChangesAsync(ct);

        await SendNoContentAsync(ct);
    }
}
