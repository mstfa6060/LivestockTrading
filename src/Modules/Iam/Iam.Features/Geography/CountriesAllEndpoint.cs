using FastEndpoints;
using Iam.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Iam.Features.Geography;

public sealed record CountriesAllRequest(string? Keyword);

public sealed record CountryItem(
    int Id,
    string Code,
    string? Code3,
    string Name,
    string? NativeName,
    string? PhoneCode,
    string DefaultCurrencyCode,
    string? DefaultCurrencySymbol
);

public sealed class CountriesAllEndpoint(IamDbContext db) : Endpoint<CountriesAllRequest, List<CountryItem>>
{
    public override void Configure()
    {
        Post("/iam/Countries/All");
        AllowAnonymous();
        Tags("Geography");
    }

    public override async Task HandleAsync(CountriesAllRequest req, CancellationToken ct)
    {
        var query = db.Countries.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(req.Keyword))
        {
            var k = req.Keyword.Trim().ToLower();
            query = query.Where(c =>
                c.Name.ToLower().Contains(k) ||
                c.Code.ToLower().Contains(k) ||
                (c.NativeName != null && c.NativeName.ToLower().Contains(k)));
        }

        var list = await query
            .OrderBy(c => c.Name)
            .Select(c => new CountryItem(
                c.Id,
                c.Code,
                c.Code3,
                c.Name,
                c.NativeName,
                c.PhoneCode,
                c.CurrencyCode,
                c.CurrencySymbol))
            .ToListAsync(ct);

        await SendAsync(list, 200, ct);
    }
}
