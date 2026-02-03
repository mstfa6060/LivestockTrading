using System;
using System.Collections.Generic;

namespace LivestockTrading.Domain.Entities;

/// <summary>
/// Ana ürün entity'si - Tüm tarımsal ürün tipleri için
/// Esnek yapı: hayvan, makine, kimyasal, tohum vb. her şeyi destekler
/// </summary>
public class Product : BaseEntity
{
    /// <summary>Ürün başlığı</summary>
    public string Title { get; set; }
    
    /// <summary>URL dostu isim</summary>
    public string Slug { get; set; }
    
    /// <summary>Detaylı açıklama</summary>
    public string Description { get; set; }
    
    /// <summary>Kısa açıklama</summary>
    public string ShortDescription { get; set; }
    
    /// <summary>Kategori ID</summary>
    public Guid CategoryId { get; set; }
    public Category Category { get; set; }
    
    /// <summary>Marka ID</summary>
    public Guid? BrandId { get; set; }
    public Brand Brand { get; set; }
    
    /// <summary>Taban fiyat</summary>
    public decimal BasePrice { get; set; }
    
    /// <summary>Para birimi (USD, EUR, TRY, vb.)</summary>
    public string Currency { get; set; }
    
    /// <summary>İndirimli fiyat</summary>
    public decimal? DiscountedPrice { get; set; }
    
    /// <summary>Fiyat birimi (adet, kg, litre, hektar başına vb.)</summary>
    public string PriceUnit { get; set; }
    
    /// <summary>Stok miktarı</summary>
    public int StockQuantity { get; set; }
    
    /// <summary>Stok birimi (adet, kg, litre vb.)</summary>
    public string StockUnit { get; set; }
    
    /// <summary>Minimum sipariş miktarı</summary>
    public int? MinOrderQuantity { get; set; }
    
    /// <summary>Maksimum sipariş miktarı</summary>
    public int? MaxOrderQuantity { get; set; }
    
    /// <summary>Stokta var mı?</summary>
    public bool IsInStock { get; set; }
    
    /// <summary>Satıcı ID</summary>
    public Guid SellerId { get; set; }
    public Seller Seller { get; set; }
    
    /// <summary>Ürün konumu ID</summary>
    public Guid LocationId { get; set; }
    public Location Location { get; set; }
    
    /// <summary>Ürün durumu</summary>
    public ProductStatus Status { get; set; }
    
    /// <summary>Ürün kondisyonu (Yeni, İkinci el, Yenilenmiş)</summary>
    public ProductCondition Condition { get; set; }
    
    /// <summary>Kargo mevcut mu?</summary>
    public bool IsShippingAvailable { get; set; }
    
    /// <summary>Kargo ücreti</summary>
    public decimal? ShippingCost { get; set; }
    
    /// <summary>Uluslararası kargo var mı?</summary>
    public bool IsInternationalShipping { get; set; }
    
    /// <summary>Ağırlık</summary>
    public decimal? Weight { get; set; }
    
    /// <summary>Ağırlık birimi</summary>
    public string WeightUnit { get; set; }
    
    /// <summary>Ürün özellikleri JSON</summary>
    public string Attributes { get; set; }
    
    /// <summary>Meta başlık (SEO)</summary>
    public string MetaTitle { get; set; }
    
    /// <summary>Meta açıklama (SEO)</summary>
    public string MetaDescription { get; set; }
    
    /// <summary>Meta anahtar kelimeler (SEO)</summary>
    public string MetaKeywords { get; set; }
    
    /// <summary>Görüntülenme sayısı</summary>
    public int ViewCount { get; set; }
    
    /// <summary>Favoriye eklenme sayısı</summary>
    public int FavoriteCount { get; set; }
    
    /// <summary>Ortalama değerlendirme puanı</summary>
    public decimal? AverageRating { get; set; }
    
    /// <summary>Yorum sayısı</summary>
    public int ReviewCount { get; set; }
    
    /// <summary>Yayınlanma tarihi</summary>
    public DateTime? PublishedAt { get; set; }
    
    /// <summary>İlan bitiş tarihi</summary>
    public DateTime? ExpiresAt { get; set; }

    /// <summary>FileProvider'daki medya bucket ID (MongoDB) - tüm foto ve videolar</summary>
    public string MediaBucketId { get; set; }

    /// <summary>Kapak görseli FileEntry.Id (FileProvider'dan)</summary>
    public string CoverImageFileId { get; set; }

    public ICollection<ProductReview> Reviews { get; set; }
    public ICollection<ProductVariant> Variants { get; set; }
    public ICollection<FavoriteProduct> Favorites { get; set; }

    public Product()
    {
        Reviews = new HashSet<ProductReview>();
        Variants = new HashSet<ProductVariant>();
        Favorites = new HashSet<FavoriteProduct>();
        Status = ProductStatus.Draft;
        Condition = ProductCondition.New;
        IsInStock = true;
    }
}

public enum ProductStatus
{
    Draft = 0,
    PendingApproval = 1,
    Active = 2,
    Inactive = 3,
    OutOfStock = 4,
    Expired = 5,
    Sold = 6,
    Rejected = 7
}

public enum ProductCondition
{
    New = 0,
    Used = 1,
    Refurbished = 2,
    ForParts = 3
}
