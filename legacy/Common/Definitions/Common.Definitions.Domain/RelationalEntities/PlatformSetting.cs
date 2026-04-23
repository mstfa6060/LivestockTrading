using System.ComponentModel.DataAnnotations.Schema;
using Common.Definitions.Base.Entity;

namespace Common.Definitions.Domain.Entities;

/// <summary>
/// Platform genelinde kullanilan dinamik ayarlar.
/// Admin panelinden yonetilir, cache'lenir.
/// </summary>
[Table("PlatformSettings")]
public class PlatformSetting : BaseEntity
{
    /// <summary>
    /// Ayar anahtari (unique) - orn: "BuyerCommissionRate"
    /// </summary>
    public string Key { get; set; }

    /// <summary>
    /// Ayar degeri (string olarak saklanir) - orn: "0.20"
    /// </summary>
    public string Value { get; set; }

    /// <summary>
    /// Veri tipi: "decimal", "int", "string", "bool"
    /// </summary>
    public string DataType { get; set; }

    /// <summary>
    /// Kategori: "Commission", "Fees", "Limits", "General"
    /// </summary>
    public string Category { get; set; }

    /// <summary>
    /// Admin panelinde gosterilecek ad - orn: "Alici Komisyon Orani"
    /// </summary>
    public string DisplayName { get; set; }

    /// <summary>
    /// Aciklama - orn: "Saticiya gosterilen tekliften dusulecek oran"
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// Siralama (admin panelinde)
    /// </summary>
    public int SortOrder { get; set; }

    /// <summary>
    /// Son guncelleyen kullanici
    /// </summary>
    public Guid? UpdatedByUserId { get; set; }

    /// <summary>
    /// Minimum deger (validasyon icin, opsiyonel)
    /// </summary>
    public string MinValue { get; set; }

    /// <summary>
    /// Maximum deger (validasyon icin, opsiyonel)
    /// </summary>
    public string MaxValue { get; set; }

    /// <summary>
    /// Salt okunur mu? (kod tarafindan yonetilen ayarlar)
    /// </summary>
    public bool IsReadOnly { get; set; }
}
