using System.ComponentModel.DataAnnotations.Schema;
using Common.Definitions.Base.Entity;

namespace Common.Definitions.Domain.Entities;

/// <summary>
/// Mobil uygulama versiyon bilgilerini tutar
/// </summary>
[Table("MobilApplicationVersiyon")]
public class MobilApplicationVersiyon : BaseEntity
{
    /// <summary>
    /// Platform: "ios" veya "android"
    /// </summary>
    public string Platform { get; set; }

    /// <summary>
    /// Minimum desteklenen versiyon (bu versiyonun altı kullanamaz)
    /// </summary>
    public string MinVersion { get; set; }

    /// <summary>
    /// Mağazadaki en güncel versiyon
    /// </summary>
    public string LatestVersion { get; set; }

    /// <summary>
    /// Zorunlu güncelleme mi?
    /// </summary>
    public bool ForceUpdate { get; set; }

    /// <summary>
    /// Kullanıcıya gösterilecek güncelleme mesajı
    /// </summary>
    public string UpdateMessage { get; set; }

    /// <summary>
    /// Mağaza linki
    /// </summary>
    public string StoreUrl { get; set; }

    /// <summary>
    /// Aktif mi?
    /// </summary>
    public bool IsActive { get; set; }
}
