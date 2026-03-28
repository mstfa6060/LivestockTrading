using System.ComponentModel.DataAnnotations.Schema;

namespace Common.Definitions.Domain.Entities;

/// <summary>
/// İlçeler / Şehirler
/// GeoNames cities15000 verileri ile doldurulur
/// </summary>
[Table("Districts")]
public class District
{
    public int Id { get; set; }

    /// <summary>
    /// İlçe/şehir adı (örn: "Kadıköy", "Houston", "München")
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Bağlı olduğu il/bölge Id
    /// </summary>
    public int ProvinceId { get; set; }

    /// <summary>
    /// Çoklu dil çevirileri JSON
    /// </summary>
    public string NameTranslations { get; set; }

    /// <summary>
    /// Enlem (GeoNames cities15000 verisinden)
    /// </summary>
    public double? Latitude { get; set; }

    /// <summary>
    /// Boylam (GeoNames cities15000 verisinden)
    /// </summary>
    public double? Longitude { get; set; }

    /// <summary>
    /// GeoNames kaynak ID (re-import için)
    /// </summary>
    public int GeoNameId { get; set; }

    /// <summary>
    /// Sıralama
    /// </summary>
    public int SortOrder { get; set; }

    // Navigation
    [ForeignKey("ProvinceId")]
    public Province Province { get; set; }

    public List<Neighborhood> Neighborhoods { get; set; } = new();
}
