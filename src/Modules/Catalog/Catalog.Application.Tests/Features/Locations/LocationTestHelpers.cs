using Shared.ValueObjects;

namespace LivestockTrading.Catalog.Application.Tests.Features.Locations;

/// <summary>
/// Shared test helpers for Location feature tests.
/// Translations fiili API: tek ctor Translations(IReadOnlyDictionary&lt;LanguageCode,string&gt;),
/// parametresiz ctor YOK, key tipi LanguageCode (W2.3 BreedTestHelpers mirror).
/// </summary>
internal static class LocationTestHelpers
{
    public static Translations EnglishText(string value) =>
        new(new Dictionary<LanguageCode, string> { [new LanguageCode("en")] = value });
}
