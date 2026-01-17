using System.ComponentModel.DataAnnotations.Schema;

namespace Common.Definitions.Domain.Entities;

/// <summary>
/// İlçeler
/// </summary>
[Table("Districts")]
public class District
{
    public int Id { get; set; }

    /// <summary>
    /// İlçe adı (örn: "Kadıköy", "Çankaya")
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Bağlı olduğu il Id
    /// </summary>
    public int ProvinceId { get; set; }

    /// <summary>
    /// Sıralama
    /// </summary>
    public int SortOrder { get; set; }

    // Navigation
    [ForeignKey("ProvinceId")]
    public Province Province { get; set; }

    public List<Neighborhood> Neighborhoods { get; set; } = new();
}
