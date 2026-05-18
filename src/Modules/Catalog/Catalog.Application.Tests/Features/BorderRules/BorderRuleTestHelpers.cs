using LivestockTrading.Catalog.Domain.Aggregates;
using Shared.ValueObjects;

namespace LivestockTrading.Catalog.Application.Tests.Features.BorderRules;

/// <summary>
/// Shared test helpers for BorderRule feature tests.
/// EnglishText: W2.4 LocationTestHelpers mirror (Translations tek ctor, LanguageCode key).
/// SampleBorderRule: Domain factory fixture (Update/Deactivate handler GetByIdAsync seam).
/// </summary>
internal static class BorderRuleTestHelpers
{
    public static Translations EnglishText(string value) =>
        new(new Dictionary<LanguageCode, string> { [new LanguageCode("en")] = value });

    public static BorderRule SampleBorderRule() =>
        BorderRule.Create(
            new CountryCode("tr"),
            new CountryCode("de"),
            BorderRuleKind.Banned,
            "{}",
            EnglishText("note"),
            Guid.NewGuid());
}
