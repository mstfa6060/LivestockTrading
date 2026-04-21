using FastEndpoints;
using Livestock.Domain.Entities;
using Livestock.Domain.Errors;
using Livestock.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Livestock.Features.Admin.Languages;

public class ListLanguagesEndpoint(LivestockDbContext db) : EndpointWithoutRequest<List<LanguageItem>>
{
    public override void Configure()
    {
        Get("/Languages");
        AllowAnonymous();
        Tags("Languages");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var languages = await db.Languages
            .AsNoTracking()
            .Where(l => !l.IsDeleted)
            .OrderBy(l => l.SortOrder).ThenBy(l => l.Name)
            .Select(l => new LanguageItem(l.Id, l.Code, l.Name, l.NativeName, l.IsRightToLeft, l.IsActive, l.IsDefault, l.SortOrder, l.FlagIconUrl, l.CreatedAt))
            .ToListAsync(ct);

        await SendAsync(languages, 200, ct);
    }
}

public class GetLanguageEndpoint(LivestockDbContext db) : Endpoint<GetLanguageRequest, LanguageItem>
{
    public override void Configure()
    {
        Get("/Languages/{Id}");
        AllowAnonymous();
        Tags("Languages");
    }

    public override async Task HandleAsync(GetLanguageRequest req, CancellationToken ct)
    {
        var l = await db.Languages.AsNoTracking().FirstOrDefaultAsync(x => x.Id == req.Id && !x.IsDeleted, ct);
        if (l is null)
        {
            AddError(LivestockErrors.LanguageErrors.LanguageNotFound);
            await SendErrorsAsync(404, ct);
            return;
        }

        await SendAsync(new LanguageItem(l.Id, l.Code, l.Name, l.NativeName, l.IsRightToLeft, l.IsActive, l.IsDefault, l.SortOrder, l.FlagIconUrl, l.CreatedAt), 200, ct);
    }
}

public class CreateLanguageEndpoint(LivestockDbContext db) : Endpoint<CreateLanguageRequest, LanguageItem>
{
    public override void Configure()
    {
        Post("/Admin/Languages");
        Roles("LivestockTrading.Admin");
        Tags("Languages");
    }

    public override async Task HandleAsync(CreateLanguageRequest req, CancellationToken ct)
    {
        var exists = await db.Languages.AnyAsync(l => l.Code == req.Code && !l.IsDeleted, ct);
        if (exists)
        {
            AddError(LivestockErrors.LanguageErrors.LanguageCodeAlreadyExists);
            await SendErrorsAsync(409, ct);
            return;
        }

        var language = new Language
        {
            Code = req.Code.ToLowerInvariant(),
            Name = req.Name,
            NativeName = req.NativeName,
            IsRightToLeft = req.IsRightToLeft,
            IsActive = req.IsActive,
            IsDefault = req.IsDefault,
            SortOrder = req.SortOrder,
            FlagIconUrl = req.FlagIconUrl
        };

        db.Languages.Add(language);
        await db.SaveChangesAsync(ct);

        await SendAsync(new LanguageItem(language.Id, language.Code, language.Name, language.NativeName, language.IsRightToLeft, language.IsActive, language.IsDefault, language.SortOrder, language.FlagIconUrl, language.CreatedAt), 201, ct);
    }
}

public class UpdateLanguageEndpoint(LivestockDbContext db) : Endpoint<UpdateLanguageRequest, LanguageItem>
{
    public override void Configure()
    {
        Put("/Admin/Languages/{Id}");
        Roles("LivestockTrading.Admin");
        Tags("Languages");
    }

    public override async Task HandleAsync(UpdateLanguageRequest req, CancellationToken ct)
    {
        var language = await db.Languages.FirstOrDefaultAsync(l => l.Id == req.Id && !l.IsDeleted, ct);
        if (language is null)
        {
            AddError(LivestockErrors.LanguageErrors.LanguageNotFound);
            await SendErrorsAsync(404, ct);
            return;
        }

        language.Name = req.Name;
        language.NativeName = req.NativeName;
        language.IsRightToLeft = req.IsRightToLeft;
        language.IsActive = req.IsActive;
        language.IsDefault = req.IsDefault;
        language.SortOrder = req.SortOrder;
        language.FlagIconUrl = req.FlagIconUrl;
        language.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync(ct);

        await SendAsync(new LanguageItem(language.Id, language.Code, language.Name, language.NativeName, language.IsRightToLeft, language.IsActive, language.IsDefault, language.SortOrder, language.FlagIconUrl, language.CreatedAt), 200, ct);
    }
}

public class DeleteLanguageEndpoint(LivestockDbContext db) : Endpoint<DeleteLanguageRequest, EmptyResponse>
{
    public override void Configure()
    {
        Delete("/Admin/Languages/{Id}");
        Roles("LivestockTrading.Admin");
        Tags("Languages");
    }

    public override async Task HandleAsync(DeleteLanguageRequest req, CancellationToken ct)
    {
        var language = await db.Languages.FirstOrDefaultAsync(l => l.Id == req.Id && !l.IsDeleted, ct);
        if (language is null)
        {
            AddError(LivestockErrors.LanguageErrors.LanguageNotFound);
            await SendErrorsAsync(404, ct);
            return;
        }

        language.IsDeleted = true;
        language.DeletedAt = DateTime.UtcNow;
        await db.SaveChangesAsync(ct);
        await SendNoContentAsync(ct);
    }
}
