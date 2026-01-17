using System.ComponentModel.DataAnnotations.Schema;

namespace Common.Definitions.Domain.Entities;

/// <summary>
/// Türkiye illeri - 81 il
/// Ortak kullanım için Common.Definitions seviyesinde
/// </summary>
[Table("Provinces")]
public class Province
{
    public int Id { get; set; }

    /// <summary>
    /// İl adı (örn: "İstanbul", "Ankara")
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Plaka kodu (örn: "34", "06")
    /// </summary>
    public string Code { get; set; }

    /// <summary>
    /// Sıralama için
    /// </summary>
    public int SortOrder { get; set; }

    // Navigation
    public List<District> Districts { get; set; } = new();
}
