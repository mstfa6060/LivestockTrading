using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Common.Definitions.Domain.Entities;

/// <summary>
/// Ülkeler - ISO 3166-1 standardı
/// Alfabetik sıralama (Name)
/// </summary>
[Table("Countries")]
public class Country
{
    public int Id { get; set; }

    /// <summary>
    /// ISO 3166-1 alpha-2 kodu (örn: "TR", "US", "DE")
    /// </summary>
    [Required]
    [MaxLength(2)]
    public string Code { get; set; }

    /// <summary>
    /// ISO 3166-1 alpha-3 kodu (örn: "TUR", "USA", "DEU")
    /// </summary>
    [Required]
    [MaxLength(3)]
    public string Code3 { get; set; }

    /// <summary>
    /// ISO 3166-1 numeric kod (örn: 792, 840, 276)
    /// </summary>
    public int NumericCode { get; set; }

    /// <summary>
    /// İngilizce ad (örn: "Turkey", "United States")
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string Name { get; set; }

    /// <summary>
    /// Yerel ad (örn: "Türkiye", "Deutschland")
    /// </summary>
    [MaxLength(100)]
    public string NativeName { get; set; }

    /// <summary>
    /// Telefon kodu (örn: "90", "1", "49") - + işareti olmadan
    /// </summary>
    [MaxLength(10)]
    public string PhoneCode { get; set; }

    /// <summary>
    /// Başkent (örn: "Ankara", "Washington D.C.")
    /// </summary>
    [MaxLength(100)]
    public string Capital { get; set; }

    /// <summary>
    /// Kıta (örn: "Europe", "Asia", "North America")
    /// </summary>
    [MaxLength(50)]
    public string Continent { get; set; }

    /// <summary>
    /// Alt bölge (örn: "Western Europe", "Southern Asia")
    /// </summary>
    [MaxLength(100)]
    public string Region { get; set; }

    /// <summary>
    /// Aktif mi (listelerde gösterilsin mi)
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Varsayılan para birimi kodu - ISO 4217 (örn: "TRY", "USD", "EUR")
    /// </summary>
    [MaxLength(3)]
    public string DefaultCurrencyCode { get; set; }

    /// <summary>
    /// Varsayılan para birimi sembolü (örn: "₺", "$", "€")
    /// </summary>
    [MaxLength(5)]
    public string DefaultCurrencySymbol { get; set; }

    // Navigation - Bu ülkedeki kullanıcılar
    public List<User> Users { get; set; } = new();
}