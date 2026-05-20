using System.Text.Json;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Shared.ValueObjects;

namespace LivestockTrading.Catalog.Infrastructure.Persistence.Conversions;

/// <summary>
/// Translations VO &lt;-&gt; jsonb string EF value converter + value comparer.
/// Translations.cs:5 Domain direktifi: "Pure POCO — STJ attribute/converter YOK;
/// JSONB map'i C3 Infra EF value converter (Domain saf)". LanguageCode private _value +
/// validating ctor (LanguageCode.cs:11-22) STJ ile doğrudan serialize edilemez →
/// ara Dictionary&lt;string,string&gt; (key = LanguageCode.Value). Round-trip ctor
/// (Translations.cs:14) + Map getter (Translations.cs:33). ValueComparer Translations.cs:35-48
/// content-equality (sıra-bağımsız) kullanır — JsonDocument REDDEDİLDİ (Adım 2: lifetime riski).
/// </summary>
public sealed class TranslationsToJsonConverter : ValueConverter<Translations, string>
{
    public TranslationsToJsonConverter()
        : base(t => Serialize(t), s => Deserialize(s))
    {
    }

    /// <summary>EF change-tracking için zorunlu — Translations referans-tip, content-equality.</summary>
    public static readonly ValueComparer<Translations> Comparer = new(
        (a, b) => (a == null && b == null) || (a != null && b != null && a.Equals(b)),
        t => t == null ? 0 : t.GetHashCode(),
        t => t == null ? Translations.Empty : new Translations(t.Map));

    /// <summary>
    /// Nullable Translations? property'ler (Description/HelpText/NativeName) için.
    /// CountryCodeConverter.Nullable emsali — mimari simetri (W3.1 build-fail F-S31 retro).
    /// HasConversion overload'unun beklediği imza: ValueConverter&lt;Translations?, string&gt;.
    /// EF nullable ref property'de null'ı kendi handle eder; converter yalnız non-null değerde.
    /// </summary>
    public static readonly ValueConverter<Translations?, string> Nullable = new(
        t => Serialize(t!),
        s => Deserialize(s));

    public static readonly ValueComparer<Translations?> NullableComparer = new(
        (a, b) => (a == null && b == null) || (a != null && b != null && a.Equals(b)),
        t => t == null ? 0 : t.GetHashCode(),
        t => t == null ? null : new Translations(t.Map));

    private static string Serialize(Translations t)
        => JsonSerializer.Serialize(
            t.Map.ToDictionary(kv => kv.Key.Value, kv => kv.Value));

    private static Translations Deserialize(string json)
        => string.IsNullOrWhiteSpace(json)
            ? Translations.Empty
            : new Translations(
                (JsonSerializer.Deserialize<Dictionary<string, string>>(json)
                 ?? new Dictionary<string, string>())
                .ToDictionary(kv => new LanguageCode(kv.Key), kv => kv.Value));
}
