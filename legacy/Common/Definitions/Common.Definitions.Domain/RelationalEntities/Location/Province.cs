using System.ComponentModel.DataAnnotations.Schema;

namespace Common.Definitions.Domain.Entities;

/// <summary>
/// Ülkelerin idari bölgeleri (il/eyalet/bölge)
/// GeoNames admin1 verileri ile doldurulur
/// </summary>
[Table("Provinces")]
public class Province
{
    public int Id { get; set; }

    /// <summary>
    /// Bağlı olduğu ülke Id
    /// </summary>
    public int CountryId { get; set; }

    /// <summary>
    /// Bölge adı (örn: "İstanbul", "Texas", "Bayern")
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// İdari kod (örn: "34", "TX", "BY")
    /// </summary>
    public string Code { get; set; }

    /// <summary>
    /// Çoklu dil çevirileri JSON (örn: {"en":"Istanbul","tr":"İstanbul","ar":"إسطنبول"})
    /// </summary>
    public string NameTranslations { get; set; }

    /// <summary>
    /// Enlem (GeoNames allCountries verisinden)
    /// </summary>
    public double? Latitude { get; set; }

    /// <summary>
    /// Boylam (GeoNames allCountries verisinden)
    /// </summary>
    public double? Longitude { get; set; }

    /// <summary>
    /// Nüfus (GeoNames allCountries verisinden)
    /// </summary>
    public long? Population { get; set; }

    /// <summary>
    /// Zaman dilimi (IANA format, ör: "Europe/Istanbul")
    /// </summary>
    public string Timezone { get; set; }

    /// <summary>
    /// GeoNames kaynak ID (re-import için)
    /// </summary>
    public int GeoNameId { get; set; }

    /// <summary>
    /// Sıralama için
    /// </summary>
    public int SortOrder { get; set; }

    // Navigation
    [ForeignKey("CountryId")]
    public Country Country { get; set; }

    public List<District> Districts { get; set; } = new();
}
