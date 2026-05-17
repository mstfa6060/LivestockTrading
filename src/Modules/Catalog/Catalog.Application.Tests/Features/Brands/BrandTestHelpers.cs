using LivestockTrading.Catalog.Domain.Aggregates;
using Shared.ValueObjects;

namespace LivestockTrading.Catalog.Application.Tests.Features.Brands;

/// <summary>
/// Shared test helpers for Brand feature tests.
/// EnglishText: W2.2 BreedTestHelpers mirror (Translations tek ctor, LanguageCode key).
/// SuggestedBrand/ApprovedBrand: heavier fixture (factory + categoryIds) — W2.2'de
/// test-class-local SampleBreed idi; Brand fixture 3 handler test dosyasında paylaşıldığı
/// için merkezi (categoryIds new[]{1} pozitif — BrandCategory ctor guard geçer).
/// </summary>
internal static class BrandTestHelpers
{
    public static Translations EnglishText(string value) =>
        new(new Dictionary<LanguageCode, string> { [new LanguageCode("en")] = value });

    public static Brand SuggestedBrand() =>
        Brand.SuggestBySeller("acme", EnglishText("Acme"), Guid.NewGuid(), new[] { 1 });

    public static Brand ApprovedBrand() =>
        Brand.CreateByAdmin("acme", EnglishText("Acme"), Guid.NewGuid(), new[] { 1 });

    /// <summary>
    /// Test-only: EF-persist tarafından atanan private-set Id property'sini in-memory
    /// unit testte pozitif değerle doldurur. Production'da DB sequence; testte handler
    /// guard'larını (BrandCategory ctor categoryId &gt; 0) geçirmek için gerekli.
    /// Sanctioned pattern (KAYDET-29): nameof + reflection, brittle-isim değil.
    /// </summary>
    internal static T WithId<T>(this T entity, int id) where T : class
    {
        var prop = typeof(T).GetProperty(nameof(Category.Id))
            ?? throw new InvalidOperationException($"{typeof(T).Name} has no 'Id' property.");
        prop.SetValue(entity, id);
        return entity;
    }
}
