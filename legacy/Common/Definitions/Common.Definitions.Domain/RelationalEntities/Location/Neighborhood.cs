using System.ComponentModel.DataAnnotations.Schema;

namespace Common.Definitions.Domain.Entities;

/// <summary>
/// Mahalleler / Köyler
/// </summary>
[Table("Neighborhoods")]
public class Neighborhood
{
    public int Id { get; set; }

    /// <summary>
    /// Mahalle/Köy adı (örn: "Caferağa Mah.")
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Bağlı olduğu ilçe Id
    /// </summary>
    public int DistrictId { get; set; }

    /// <summary>
    /// Posta kodu (varsa)
    /// </summary>
    public string PostalCode { get; set; }

    /// <summary>
    /// Enlem
    /// </summary>
    public double? Latitude { get; set; }

    /// <summary>
    /// Boylam
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
    [ForeignKey("DistrictId")]
    public District District { get; set; }
}
