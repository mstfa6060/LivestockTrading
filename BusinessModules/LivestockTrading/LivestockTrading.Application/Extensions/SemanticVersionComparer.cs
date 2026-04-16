using System;

namespace LivestockTrading.Application.Extensions;

/// <summary>
/// Basit semantic version karşılaştırıcı. "1.2.3" biçimindeki üç parçalı
/// sürümleri major.minor.patch olarak ayrıştırır ve -1/0/1 döndürür.
/// Geçersiz girişler "0.0.0" olarak değerlendirilir.
/// </summary>
public static class SemanticVersionComparer
{
    /// <summary>
    /// "a" sürümü "b" sürümünden küçükse -1, eşitse 0, büyükse 1.
    /// </summary>
    public static int Compare(string a, string b)
    {
        var (am, an, ap) = Parse(a);
        var (bm, bn, bp) = Parse(b);

        if (am != bm) return am < bm ? -1 : 1;
        if (an != bn) return an < bn ? -1 : 1;
        if (ap != bp) return ap < bp ? -1 : 1;
        return 0;
    }

    private static (int major, int minor, int patch) Parse(string version)
    {
        if (string.IsNullOrWhiteSpace(version))
            return (0, 0, 0);

        // Olasi "v" on eki veya "-beta" gibi suffix'leri ayikla
        var trimmed = version.Trim().TrimStart('v', 'V');
        var dashIndex = trimmed.IndexOf('-');
        if (dashIndex >= 0) trimmed = trimmed.Substring(0, dashIndex);

        var parts = trimmed.Split('.');
        int major = 0, minor = 0, patch = 0;
        if (parts.Length > 0) int.TryParse(parts[0], out major);
        if (parts.Length > 1) int.TryParse(parts[1], out minor);
        if (parts.Length > 2) int.TryParse(parts[2], out patch);
        return (major, minor, patch);
    }
}
