using Shared.ValueObjects;

namespace LivestockTrading.Catalog.Application.Tests.Features.CertificationTypes;

/// <summary>
/// Shared test helpers for CertificationType feature tests.
/// Translations fiili API: tek ctor Translations(IReadOnlyDictionary&lt;LanguageCode,string&gt;),
/// parametresiz ctor YOK, key tipi LanguageCode (W2.4 LocationTestHelpers mirror).
/// </summary>
internal static class CertificationTypeTestHelpers
{
    public static Translations EnglishText(string value) =>
        new(new Dictionary<LanguageCode, string> { [new LanguageCode("en")] = value });
}
