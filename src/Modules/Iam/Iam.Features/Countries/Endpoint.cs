using FastEndpoints;
using Iam.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Iam.Features.Countries;

public sealed class GetCountriesEndpoint(IamDbContext db) : EndpointWithoutRequest<List<CountryItem>>
{
    public override void Configure()
    {
        Get("/Countries");
        AllowAnonymous();
        Tags("Countries");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var countries = await db.Countries
            .AsNoTracking()
            .OrderBy(c => c.Name)
            .Select(c => new CountryItem(c.Id, c.Name, c.Code, c.CurrencyCode, c.CurrencySymbol))
            .ToListAsync(ct);

        await SendAsync(countries, 200, ct);
    }
}
