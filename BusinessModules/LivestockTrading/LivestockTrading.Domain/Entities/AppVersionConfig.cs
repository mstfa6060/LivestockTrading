using System;

namespace LivestockTrading.Domain.Entities;

/// <summary>
/// Platform bazında desteklenen uygulama sürümü konfigürasyonu.
/// Mobil/web istemcileri başlangıçta /AppVersions/Check çağrısı ile kendi
/// sürümlerini bu kayda göre değerlendirir.
/// </summary>
public class AppVersionConfig : BaseEntity
{
    /// <summary>Platform: 0=Web, 1=Android, 2=iOS (matches Platform enum used in JWT claims)</summary>
    public int Platform { get; set; }

    /// <summary>Minimum supported version (semantic version e.g. "1.0.0"). Below this triggers ForceUpdate.</summary>
    public string MinSupportedVersion { get; set; }

    /// <summary>Latest released version (semantic version e.g. "1.2.5"). Below this triggers SoftUpdate.</summary>
    public string LatestVersion { get; set; }

    /// <summary>Store URL (Play Store / App Store / web URL)</summary>
    public string StoreUrl { get; set; }

    /// <summary>Optional update message shown to user (short, translatable by frontend via key)</summary>
    public string UpdateMessage { get; set; }

    /// <summary>If true, below-minimum versions are blocked entirely.</summary>
    public bool IsActive { get; set; } = true;
}
