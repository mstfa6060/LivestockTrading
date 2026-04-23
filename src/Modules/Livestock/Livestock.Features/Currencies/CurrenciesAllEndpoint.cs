using FastEndpoints;
using Livestock.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Livestock.Features.Currencies;

// Legacy alias for frontend callers (useCurrencies hook) which were coded
// against an older backend that exposed currencies at a non-admin path.
// Returns the same shape as /livestocktrading/Admin/Currencies/All but
// anonymous and only active currencies.
public sealed record CurrenciesAllRequest;

public sealed record CurrencyItem(
    Guid Id,
    string Code,
    string Symbol,
    string Name,
    decimal ExchangeRateToUsd,
    DateTime? LastUpdated,
    bool IsActive,
    DateTime CreatedAt
);

public sealed class CurrenciesAllEndpoint(LivestockDbContext db)
    : EndpointWithoutRequest<List<CurrencyItem>>
{
    public override void Configure()
    {
        Post("/livestocktrading/Currencies/All");
        AllowAnonymous();
        Tags("Currencies");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var list = await db.Currencies
            .AsNoTracking()
            .Where(c => c.IsActive)
            .OrderBy(c => c.Code)
            .Select(c => new CurrencyItem(
                c.Id,
                c.Code,
                c.Symbol,
                c.Name,
                c.ExchangeRateToUsd,
                c.LastUpdatedAt,
                c.IsActive,
                c.CreatedAt))
            .ToListAsync(ct);

        await SendAsync(list, 200, ct);
    }
}
