using System;
using System.Collections.Generic;

namespace LivestockTrading.Domain.Entities;

/// <summary>
/// Ürün kategorileri - Hiyerarşik yapı
/// Sınırsız derinlikte alt kategori desteği
/// </summary>
public class Category : BaseEntity
{
    /// <summary>Kategori adı</summary>
    public string Name { get; set; }
    
    /// <summary>URL dostu isim</summary>
    public string Slug { get; set; }
    
    /// <summary>Kategori açıklaması</summary>
    public string Description { get; set; }
    
    /// <summary>Kategori ikonu URL (FileProvider'dan)</summary>
    public string IconUrl { get; set; }
    
    /// <summary>Sıralama</summary>
    public int SortOrder { get; set; }
    
    /// <summary>Aktif mi?</summary>
    public bool IsActive { get; set; }
    
    // Hiyerarşik yapı
    /// <summary>Üst kategori ID</summary>
    public Guid? ParentCategoryId { get; set; }
    
    /// <summary>Üst kategori</summary>
    public Category ParentCategory { get; set; }
    
    /// <summary>Alt kategoriler</summary>
    public ICollection<Category> SubCategories { get; set; }
    
    // Çoklu dil desteği
    /// <summary>Çeviri JSON: {"en":"Livestock","tr":"Hayvancılık","de":"Viehzucht"}</summary>
    public string NameTranslations { get; set; }
    
    /// <summary>Açıklama çevirileri JSON</summary>
    public string DescriptionTranslations { get; set; }
    
    // Kategori-özel özellik şablonu
    /// <summary>Bu kategoriye ait ürünlerin özellik şablonu (JSON)</summary>
    public string AttributesTemplate { get; set; }
    
    // İlişkiler
    /// <summary>Bu kategorideki ürünler</summary>
    public ICollection<Product> Products { get; set; }
    
    public Category()
    {
        SubCategories = new HashSet<Category>();
        Products = new HashSet<Product>();
        IsActive = true;
    }
}
