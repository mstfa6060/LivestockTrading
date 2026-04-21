using FastEndpoints;
using Livestock.Domain.Entities;
using Livestock.Domain.Errors;
using Livestock.Persistence;
using Microsoft.EntityFrameworkCore;
using Shared.Abstractions.Identity;

namespace Livestock.Features.ContactForms;

public class SubmitContactFormEndpoint(LivestockDbContext db, IUserContext user) : Endpoint<CreateContactFormRequest, ContactFormItem>
{
    public override void Configure()
    {
        Post("/ContactForms");
        AllowAnonymous();
        Tags("ContactForms");
    }

    public override async Task HandleAsync(CreateContactFormRequest req, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(req.Name))
        {
            AddError(x => x.Name, LivestockErrors.ContactFormErrors.ContactFormNameRequired);
            await SendErrorsAsync(400, ct);
            return;
        }

        if (string.IsNullOrWhiteSpace(req.Email))
        {
            AddError(x => x.Email, LivestockErrors.ContactFormErrors.ContactFormEmailRequired);
            await SendErrorsAsync(400, ct);
            return;
        }

        if (string.IsNullOrWhiteSpace(req.Message))
        {
            AddError(x => x.Message, LivestockErrors.ContactFormErrors.ContactFormMessageRequired);
            await SendErrorsAsync(400, ct);
            return;
        }

        Guid? userId = null;
        try { userId = user.UserId; } catch { }

        var form = new ContactForm
        {
            Name = req.Name,
            Email = req.Email,
            Subject = req.Subject,
            Message = req.Message,
            UserId = userId
        };

        db.ContactForms.Add(form);
        await db.SaveChangesAsync(ct);

        await SendAsync(new ContactFormItem(form.Id, form.Name, form.Email, form.Subject, form.Message, form.IsRead, form.UserId, form.CreatedAt), 201, ct);
    }
}

public class ListContactFormsEndpoint(LivestockDbContext db) : EndpointWithoutRequest<List<ContactFormItem>>
{
    public override void Configure()
    {
        Get("/Admin/ContactForms");
        Roles("LivestockTrading.Admin", "LivestockTrading.Moderator");
        Tags("ContactForms");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var forms = await db.ContactForms
            .AsNoTracking()
            .Where(f => !f.IsDeleted)
            .OrderByDescending(f => f.CreatedAt)
            .Select(f => new ContactFormItem(f.Id, f.Name, f.Email, f.Subject, f.Message, f.IsRead, f.UserId, f.CreatedAt))
            .ToListAsync(ct);

        await SendAsync(forms, 200, ct);
    }
}

public class GetContactFormEndpoint(LivestockDbContext db) : Endpoint<GetContactFormRequest, ContactFormItem>
{
    public override void Configure()
    {
        Get("/Admin/ContactForms/{Id}");
        Roles("LivestockTrading.Admin", "LivestockTrading.Moderator");
        Tags("ContactForms");
    }

    public override async Task HandleAsync(GetContactFormRequest req, CancellationToken ct)
    {
        var f = await db.ContactForms.AsNoTracking().FirstOrDefaultAsync(x => x.Id == req.Id && !x.IsDeleted, ct);
        if (f is null)
        {
            AddError(LivestockErrors.ContactFormErrors.ContactFormNotFound);
            await SendErrorsAsync(404, ct);
            return;
        }

        await SendAsync(new ContactFormItem(f.Id, f.Name, f.Email, f.Subject, f.Message, f.IsRead, f.UserId, f.CreatedAt), 200, ct);
    }
}

public class MarkContactFormReadEndpoint(LivestockDbContext db) : Endpoint<MarkContactFormReadRequest, EmptyResponse>
{
    public override void Configure()
    {
        Post("/Admin/ContactForms/{Id}/MarkRead");
        Roles("LivestockTrading.Admin", "LivestockTrading.Moderator");
        Tags("ContactForms");
    }

    public override async Task HandleAsync(MarkContactFormReadRequest req, CancellationToken ct)
    {
        var form = await db.ContactForms.FirstOrDefaultAsync(f => f.Id == req.Id && !f.IsDeleted, ct);
        if (form is null)
        {
            AddError(LivestockErrors.ContactFormErrors.ContactFormNotFound);
            await SendErrorsAsync(404, ct);
            return;
        }

        form.IsRead = true;
        form.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync(ct);
        await SendNoContentAsync(ct);
    }
}

public class DeleteContactFormEndpoint(LivestockDbContext db) : Endpoint<DeleteContactFormRequest, EmptyResponse>
{
    public override void Configure()
    {
        Delete("/Admin/ContactForms/{Id}");
        Roles("LivestockTrading.Admin");
        Tags("ContactForms");
    }

    public override async Task HandleAsync(DeleteContactFormRequest req, CancellationToken ct)
    {
        var form = await db.ContactForms.FirstOrDefaultAsync(f => f.Id == req.Id && !f.IsDeleted, ct);
        if (form is null)
        {
            AddError(LivestockErrors.ContactFormErrors.ContactFormNotFound);
            await SendErrorsAsync(404, ct);
            return;
        }

        form.IsDeleted = true;
        form.DeletedAt = DateTime.UtcNow;
        await db.SaveChangesAsync(ct);
        await SendNoContentAsync(ct);
    }
}
