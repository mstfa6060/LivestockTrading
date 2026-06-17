using FastEndpoints;
using Livestock.Domain.Entities;
using Livestock.Domain.Errors;
using Livestock.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Livestock.Features.Admin.Currencies;

public class ListCurrenciesAdminEndpoint(LivestockDbContext db) : EndpointWithoutRequest<List<CurrencyAdminItem>>
{
    public override void Configure()
    {
        Post("/livestocktrading/Admin/Currencies/All");
        Roles("LivestockTrading.Admin");
        Tags("Admin");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var currencies = await db.Currencies
            .AsNoTracking()
            .OrderBy(c => c.Code)
            .Select(c => new CurrencyAdminItem(c.Id, c.Code, c.Name, c.Symbol, c.IsActive, c.ExchangeRateToUsd, c.LastUpdatedAt, c.CreatedAt))
            .ToListAsync(ct);

        await SendAsync(currencies, 200, ct);
    }
}

public class GetCurrencyAdminEndpoint(LivestockDbContext db) : Endpoint<GetCurrencyAdminRequest, CurrencyAdminItem>
{
    public override void Configure()
    {
        Post("/livestocktrading/Admin/Currencies/Detail");
        Roles("LivestockTrading.Admin");
        Tags("Admin");
    }

    public override async Task HandleAsync(GetCurrencyAdminRequest req, CancellationToken ct)
    {
        var c = await db.Currencies.AsNoTracking().FirstOrDefaultAsync(x => x.Id == req.Id, ct);
        if (c is null)
        {
            AddError(LivestockErrors.CurrencyErrors.CurrencyNotFound);
            await SendErrorsAsync(404, ct);
            return;
        }

        await SendAsync(new CurrencyAdminItem(c.Id, c.Code, c.Name, c.Symbol, c.IsActive, c.ExchangeRateToUsd, c.LastUpdatedAt, c.CreatedAt), 200, ct);
    }
}

public class CreateCurrencyEndpoint(LivestockDbContext db) : Endpoint<CreateCurrencyRequest, CurrencyAdminItem>
{
    public override void Configure()
    {
        Post("/livestocktrading/Admin/Currencies/Create");
        Roles("LivestockTrading.Admin");
        Tags("Admin");
    }

    public override async Task HandleAsync(CreateCurrencyRequest req, CancellationToken ct)
    {
        var exists = await db.Currencies.AnyAsync(c => c.Code == req.Code, ct);
        if (exists)
        {
            AddError(LivestockErrors.CurrencyErrors.CurrencyCodeAlreadyExists);
            await SendErrorsAsync(409, ct);
            return;
        }

        var currency = new Currency
        {
            Code = req.Code.ToUpperInvariant(),
            Name = req.Name,
            Symbol = req.Symbol,
            ExchangeRateToUsd = req.ExchangeRateToUsd,
            LastUpdatedAt = DateTime.UtcNow
        };

        db.Currencies.Add(currency);
        await db.SaveChangesAsync(ct);

        await SendAsync(new CurrencyAdminItem(currency.Id, currency.Code, currency.Name, currency.Symbol, currency.IsActive, currency.ExchangeRateToUsd, currency.LastUpdatedAt, currency.CreatedAt), 201, ct);
    }
}

public class UpdateCurrencyEndpoint(LivestockDbContext db) : Endpoint<UpdateCurrencyRequest, CurrencyAdminItem>
{
    public override void Configure()
    {
        Post("/livestocktrading/Admin/Currencies/Update");
        Roles("LivestockTrading.Admin");
        Tags("Admin");
    }

    public override async Task HandleAsync(UpdateCurrencyRequest req, CancellationToken ct)
    {
        var currency = await db.Currencies.FirstOrDefaultAsync(c => c.Id == req.Id, ct);
        if (currency is null)
        {
            AddError(LivestockErrors.CurrencyErrors.CurrencyNotFound);
            await SendErrorsAsync(404, ct);
            return;
        }

        currency.Name = req.Name;
        currency.Symbol = req.Symbol;
        currency.IsActive = req.IsActive;
        currency.ExchangeRateToUsd = req.ExchangeRateToUsd;
        currency.LastUpdatedAt = DateTime.UtcNow;
        currency.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync(ct);

        await SendAsync(new CurrencyAdminItem(currency.Id, currency.Code, currency.Name, currency.Symbol, currency.IsActive, currency.ExchangeRateToUsd, currency.LastUpdatedAt, currency.CreatedAt), 200, ct);
    }
}

public class DeleteCurrencyEndpoint(LivestockDbContext db) : Endpoint<DeleteCurrencyRequest, EmptyResponse>
{
    public override void Configure()
    {
        Post("/livestocktrading/Admin/Currencies/Delete");
        Roles("LivestockTrading.Admin");
        Tags("Admin");
    }

    public override async Task HandleAsync(DeleteCurrencyRequest req, CancellationToken ct)
    {
        var currency = await db.Currencies.FirstOrDefaultAsync(c => c.Id == req.Id, ct);
        if (currency is null)
        {
            AddError(LivestockErrors.CurrencyErrors.CurrencyNotFound);
            await SendErrorsAsync(404, ct);
            return;
        }

        db.Currencies.Remove(currency);
        await db.SaveChangesAsync(ct);
        await SendNoContentAsync(ct);
    }
}
