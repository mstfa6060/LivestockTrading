namespace Shared.Text;

using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

/// <summary>"John Doe Çiftliği" → "john-doe-ciftligi". TR-char map + Unicode FormD + kebab-case (MaxLength 80).</summary>
public static partial class SlugHelper
{
    private const int MaxLength = 80;

    private static readonly Dictionary<char, char> TrMap = new()
    {
        ['ı'] = 'i', ['İ'] = 'i', ['ş'] = 's', ['Ş'] = 's', ['ç'] = 'c', ['Ç'] = 'c',
        ['ğ'] = 'g', ['Ğ'] = 'g', ['ü'] = 'u', ['Ü'] = 'u', ['ö'] = 'o', ['Ö'] = 'o',
    };

    public static string Normalize(string input)
    {
        if (string.IsNullOrWhiteSpace(input)) return string.Empty;

        var sb = new StringBuilder(input.Length);
        foreach (var ch in input)
            sb.Append(TrMap.TryGetValue(ch, out var mapped) ? mapped : ch);

        var formD = sb.ToString().Normalize(NormalizationForm.FormD);
        var noDiacritics = new string(formD
            .Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
            .ToArray());

        var lower = noDiacritics.ToLowerInvariant();
        var slug = NonAlnum().Replace(lower, "-").Trim('-');
        slug = MultiDash().Replace(slug, "-");
        return slug.Length > MaxLength ? slug[..MaxLength].Trim('-') : slug;
    }

    [GeneratedRegex("[^a-z0-9]+")] private static partial Regex NonAlnum();
    [GeneratedRegex("-{2,}")] private static partial Regex MultiDash();
}
