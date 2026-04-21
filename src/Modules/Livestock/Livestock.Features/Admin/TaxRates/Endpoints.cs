using FastEndpoints;
using Livestock.Domain.Entities;
using Livestock.Domain.Errors;
using Livestock.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Livestock.Features.Admin.TaxRates;

public class ListTaxRatesEndpoint(LivestockDbContext db) : EndpointWithoutRequest<List<TaxRateItem>>
{
    public override void Configure()
    {
        Get("/Admin/TaxRates");
        Roles("LivestockTrading.Admin", "LivestockTrading.Moderator");
        Tags("Admin", "TaxRates");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var rates = await db.TaxRates
            .AsNoTracking()
            .Where(t => !t.IsDeleted)
            .OrderBy(t => t.CountryCode).ThenBy(t => t.TaxName)
            .Select(t => new TaxRateItem(t.Id, t.CountryCode, t.StateCode, t.TaxName, t.Rate, t.Type, t.CategoryId, t.IsActive, t.ValidFrom, t.ValidUntil, t.CreatedAt))
            .ToListAsync(ct);

        await SendAsync(rates, 200, ct);
    }
}

public class GetTaxRatesByCountryEndpoint(LivestockDbContext db) : Endpoint<GetTaxRatesByCountryRequest, List<TaxRateItem>>
{
    public override void Configure()
    {
        Get("/TaxRates/ByCountry/{CountryCode}");
        AllowAnonymous();
        Tags("TaxRates");
    }

    public override async Task HandleAsync(GetTaxRatesByCountryRequest req, CancellationToken ct)
    {
        var rates = await db.TaxRates
            .AsNoTracking()
            .Where(t => t.CountryCode == req.CountryCode && t.IsActive && !t.IsDeleted)
            .OrderBy(t => t.TaxName)
            .Select(t => new TaxRateItem(t.Id, t.CountryCode, t.StateCode, t.TaxName, t.Rate, t.Type, t.CategoryId, t.IsActive, t.ValidFrom, t.ValidUntil, t.CreatedAt))
            .ToListAsync(ct);

        await SendAsync(rates, 200, ct);
    }
}

public class GetTaxRateEndpoint(LivestockDbContext db) : Endpoint<GetTaxRateRequest, TaxRateItem>
{
    public override void Configure()
    {
        Get("/Admin/TaxRates/{Id}");
        Roles("LivestockTrading.Admin", "LivestockTrading.Moderator");
        Tags("Admin", "TaxRates");
    }

    public override async Task HandleAsync(GetTaxRateRequest req, CancellationToken ct)
    {
        var t = await db.TaxRates.AsNoTracking().FirstOrDefaultAsync(x => x.Id == req.Id && !x.IsDeleted, ct);
        if (t is null)
        {
            AddError(LivestockErrors.TaxRateErrors.TaxRateNotFound);
            await SendErrorsAsync(404, ct);
            return;
        }

        await SendAsync(new TaxRateItem(t.Id, t.CountryCode, t.StateCode, t.TaxName, t.Rate, t.Type, t.CategoryId, t.IsActive, t.ValidFrom, t.ValidUntil, t.CreatedAt), 200, ct);
    }
}

public class CreateTaxRateEndpoint(LivestockDbContext db) : Endpoint<CreateTaxRateRequest, TaxRateItem>
{
    public override void Configure()
    {
        Post("/Admin/TaxRates");
        Roles("LivestockTrading.Admin");
        Tags("Admin", "TaxRates");
    }

    public override async Task HandleAsync(CreateTaxRateRequest req, CancellationToken ct)
    {
        var taxRate = new TaxRate
        {
            CountryCode = req.CountryCode.ToUpperInvariant(),
            StateCode = req.StateCode,
            TaxName = req.TaxName,
            Rate = req.Rate,
            Type = req.Type,
            CategoryId = req.CategoryId,
            IsActive = req.IsActive,
            ValidFrom = req.ValidFrom,
            ValidUntil = req.ValidUntil
        };

        db.TaxRates.Add(taxRate);
        await db.SaveChangesAsync(ct);

        await SendAsync(new TaxRateItem(taxRate.Id, taxRate.CountryCode, taxRate.StateCode, taxRate.TaxName, taxRate.Rate, taxRate.Type, taxRate.CategoryId, taxRate.IsActive, taxRate.ValidFrom, taxRate.ValidUntil, taxRate.CreatedAt), 201, ct);
    }
}

public class UpdateTaxRateEndpoint(LivestockDbContext db) : Endpoint<UpdateTaxRateRequest, TaxRateItem>
{
    public override void Configure()
    {
        Put("/Admin/TaxRates/{Id}");
        Roles("LivestockTrading.Admin");
        Tags("Admin", "TaxRates");
    }

    public override async Task HandleAsync(UpdateTaxRateRequest req, CancellationToken ct)
    {
        var taxRate = await db.TaxRates.FirstOrDefaultAsync(t => t.Id == req.Id && !t.IsDeleted, ct);
        if (taxRate is null)
        {
            AddError(LivestockErrors.TaxRateErrors.TaxRateNotFound);
            await SendErrorsAsync(404, ct);
            return;
        }

        taxRate.CountryCode = req.CountryCode.ToUpperInvariant();
        taxRate.StateCode = req.StateCode;
        taxRate.TaxName = req.TaxName;
        taxRate.Rate = req.Rate;
        taxRate.Type = req.Type;
        taxRate.CategoryId = req.CategoryId;
        taxRate.IsActive = req.IsActive;
        taxRate.ValidFrom = req.ValidFrom;
        taxRate.ValidUntil = req.ValidUntil;
        taxRate.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync(ct);

        await SendAsync(new TaxRateItem(taxRate.Id, taxRate.CountryCode, taxRate.StateCode, taxRate.TaxName, taxRate.Rate, taxRate.Type, taxRate.CategoryId, taxRate.IsActive, taxRate.ValidFrom, taxRate.ValidUntil, taxRate.CreatedAt), 200, ct);
    }
}

public class DeleteTaxRateEndpoint(LivestockDbContext db) : Endpoint<DeleteTaxRateRequest, EmptyResponse>
{
    public override void Configure()
    {
        Delete("/Admin/TaxRates/{Id}");
        Roles("LivestockTrading.Admin");
        Tags("Admin", "TaxRates");
    }

    public override async Task HandleAsync(DeleteTaxRateRequest req, CancellationToken ct)
    {
        var taxRate = await db.TaxRates.FirstOrDefaultAsync(t => t.Id == req.Id && !t.IsDeleted, ct);
        if (taxRate is null)
        {
            AddError(LivestockErrors.TaxRateErrors.TaxRateNotFound);
            await SendErrorsAsync(404, ct);
            return;
        }

        taxRate.IsDeleted = true;
        taxRate.DeletedAt = DateTime.UtcNow;
        await db.SaveChangesAsync(ct);
        await SendNoContentAsync(ct);
    }
}
