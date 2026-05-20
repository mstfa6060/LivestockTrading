using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Shared.ValueObjects;

namespace LivestockTrading.Catalog.Infrastructure.Persistence.Conversions;

/// <summary>
/// CountryCode VO &lt;-&gt; ISO 3166-1 alpha-2 string EF value converters.
/// CountryCode = readonly record struct, tek string Value, default(CountryCode) → fail-fast
/// (CountryCode.cs:13). 4 kullanım DRY (Açık Karar 6): BorderRule.From/ToCountry +
/// Location.CountryCode (non-null) · Brand.OriginCountry (nullable). Ctor doğrulaması
/// (CountryCode.cs:16-22) DB→model dönüşünde yeniden uygulanır (tek fail-fast noktası).
/// </summary>
public static class CountryCodeConverter
{
    public static readonly ValueConverter<CountryCode, string> NonNull = new(
        c => c.Value,
        s => new CountryCode(s));

    public static readonly ValueConverter<CountryCode?, string?> Nullable = new(
        c => c.HasValue ? c.Value.Value : null,
        s => s == null ? (CountryCode?)null : new CountryCode(s));
}
