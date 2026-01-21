using System;
using System.Collections.Generic;

namespace LivestockTrading.Domain.Entities;

/// <summary>
/// Marka/Üretici bilgileri
/// </summary>
public class Brand : BaseEntity
{
    /// <summary>Marka adı</summary>
    public string Name { get; set; }
    /// <summary>URL dostu isim</summary>
    public string Slug { get; set; }
    /// <summary>Marka açıklaması</summary>
    public string Description { get; set; }
    /// <summary>Logo URL (FileProvider'dan)</summary>
    public string LogoUrl { get; set; }
    /// <summary>Web sitesi</summary>
    public string Website { get; set; }
    /// <summary>E-posta</summary>
    public string Email { get; set; }
    /// <summary>Telefon</summary>
    public string Phone { get; set; }
    /// <summary>Menşei ülke kodu (ISO 3166-1 alpha-2)</summary>
    public string CountryCode { get; set; }
    /// <summary>Aktif mi?</summary>
    public bool IsActive { get; set; }
    /// <summary>Doğrulanmış mı?</summary>
    public bool IsVerified { get; set; }
    
    public ICollection<Product> Products { get; set; }
    
    public Brand()
    {
        Products = new HashSet<Product>();
        IsActive = true;
        IsVerified = false;
    }
}