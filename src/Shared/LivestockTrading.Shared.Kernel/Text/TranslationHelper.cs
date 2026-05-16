namespace Shared.Text;

using Shared.ValueObjects;

/// <summary>05-catalog §9 birebir. Çok-dilli metin çözümleme: preferred → ignore-case → customFallback → en → ilk.</summary>
public static class TranslationHelper
{
    public const string FallbackLocale = "en";

    public static string Resolve(Translations translations, string preferredLocale, string? customFallback = null)
    {
        if (translations is null || translations.IsEmpty) return string.Empty;

        if (translations.TryGet(preferredLocale, out var v) && !string.IsNullOrWhiteSpace(v))
            return v;
        if (translations.TryGetIgnoreCase(preferredLocale, out v) && !string.IsNullOrWhiteSpace(v))
            return v;
        if (customFallback != null && translations.TryGet(customFallback, out v) && !string.IsNullOrWhiteSpace(v))
            return v;
        if (translations.TryGet(FallbackLocale, out v) && !string.IsNullOrWhiteSpace(v))
            return v;
        return translations.FirstOrEmpty();
    }
}
