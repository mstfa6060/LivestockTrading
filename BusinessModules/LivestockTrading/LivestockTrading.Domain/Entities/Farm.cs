using System;
using System.Collections.Generic;

namespace LivestockTrading.Domain.Entities;

/// <summary>
/// Çiftlik/Tarımsal tesis bilgileri
/// </summary>
public class Farm : BaseEntity
{
    /// <summary>Çiftlik adı</summary>
    public string Name { get; set; }
    /// <summary>Açıklama</summary>
    public string Description { get; set; }
    /// <summary>Kayıt numarası</summary>
    public string RegistrationNumber { get; set; }
    /// <summary>Satıcı ID</summary>
    public Guid SellerId { get; set; }
    public Seller Seller { get; set; }
    /// <summary>Konum ID</summary>
    public Guid LocationId { get; set; }
    public Location Location { get; set; }
    /// <summary>Çiftlik tipi</summary>
    public FarmType Type { get; set; }
    /// <summary>Toplam alan (hektar)</summary>
    public decimal? TotalAreaHectares { get; set; }
    /// <summary>Ekilebilir alan (hektar)</summary>
    public decimal? CultivatedAreaHectares { get; set; }
    /// <summary>Sertifikalar JSON: ["Organik","GAP","Helal"]</summary>
    public string Certifications { get; set; }
    /// <summary>Organik mi?</summary>
    public bool IsOrganic { get; set; }
    /// <summary>Görsel URL'leri JSON (FileProvider'dan)</summary>
    public string ImageUrls { get; set; }
    /// <summary>Video URL (FileProvider'dan)</summary>
    public string VideoUrl { get; set; }
    /// <summary>Aktif mi?</summary>
    public bool IsActive { get; set; }
    /// <summary>Doğrulanmış mı?</summary>
    public bool IsVerified { get; set; }
    
    public ICollection<Product> Products { get; set; }
    
    public Farm()
    {
        Products = new HashSet<Product>();
        IsActive = true;
        IsVerified = false;
    }
}

public enum FarmType
{
    Livestock = 0,
    Crop = 1,
    Dairy = 2,
    Poultry = 3,
    Aquaculture = 4,
    Greenhouse = 5,
    Vineyard = 6,
    Orchard = 7,
    MixedFarming = 8,
    Other = 99
}
