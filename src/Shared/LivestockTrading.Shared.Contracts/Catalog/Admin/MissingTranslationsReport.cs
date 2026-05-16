namespace Shared.Contracts.Catalog.Admin;

/// <summary>§9:869-876 GET /admin/catalog/translations/missing?locale=. Per-locale scoped, bucket-per-entity eksik çeviri raporu.</summary>
public sealed record MissingTranslationsReport(
    string Locale,
    IReadOnlyList<MissingTranslationItem> Categories,
    IReadOnlyList<MissingTranslationItem> Breeds,
    IReadOnlyList<MissingTranslationItem> Brands,
    IReadOnlyList<MissingTranslationItem> Certifications);

/// <summary>Eksik çeviri kalemi (context-spesifik companion, aynı dosya). Code = dış kimlik; FallbackNamePreview = mevcut (genelde en) ad, admin UI bağlamı.</summary>
public sealed record MissingTranslationItem(
    string Code,
    string? FallbackNamePreview);
