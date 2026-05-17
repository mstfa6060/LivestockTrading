using Shared.ValueObjects;

namespace LivestockTrading.Catalog.Application.Tests.Features.Categories;

/// <summary>
/// Shared test helpers for Category feature tests.
/// Translations fiili API: tek ctor Translations(IReadOnlyDictionary&lt;LanguageCode,string&gt;),
/// parametresiz ctor YOK, key tipi LanguageCode (W2.1 reflex grep teyit).
/// </summary>
internal static class CategoryTestHelpers
{
    public static Translations EnglishText(string value) =>
        new(new Dictionary<LanguageCode, string> { [new LanguageCode("en")] = value });
}
