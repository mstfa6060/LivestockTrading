using System;
using System.Collections.Generic;

namespace LivestockTrading.Domain.Entities;

/// <summary>
/// Ürün değerlendirmesi/yorumu
/// </summary>
public class ProductReview : BaseEntity
{
    public Guid ProductId { get; set; }
    public Product Product { get; set; }
    /// <summary>Değerlendiren kullanıcı ID</summary>
    public Guid UserId { get; set; }
    /// <summary>Sipariş ID (doğrulanmış alım)</summary>
    public Guid? OrderId { get; set; }
    /// <summary>Değerlendirme (1-5 yıldız)</summary>
    public int Rating { get; set; }
    /// <summary>Başlık</summary>
    public string Title { get; set; }
    /// <summary>Yorum</summary>
    public string Comment { get; set; }
    /// <summary>Doğrulanmış alım mı?</summary>
    public bool IsVerifiedPurchase { get; set; }
    /// <summary>Onaylandı mı?</summary>
    public bool IsApproved { get; set; }
    /// <summary>Onay tarihi</summary>
    public DateTime? ApprovedAt { get; set; }
    /// <summary>Onaylayan kullanıcı ID</summary>
    public Guid? ApprovedByUserId { get; set; }
    /// <summary>Faydalı bulma sayısı</summary>
    public int HelpfulCount { get; set; }
    /// <summary>Faydalı bulmama sayısı</summary>
    public int NotHelpfulCount { get; set; }
    /// <summary>Satıcı yanıtı</summary>
    public string SellerResponse { get; set; }
    /// <summary>Satıcı yanıt tarihi</summary>
    public DateTime? SellerRespondedAt { get; set; }
    /// <summary>Görsel URL'leri JSON (FileProvider'dan)</summary>
    public string ImageUrls { get; set; }
    /// <summary>Video URL'leri JSON (FileProvider'dan)</summary>
    public string VideoUrls { get; set; }
    
    public ProductReview()
    {
        IsApproved = false;
        IsVerifiedPurchase = false;
    }
}

/// <summary>
/// Satıcı değerlendirmesi
/// </summary>
public class SellerReview : BaseEntity
{
    public Guid SellerId { get; set; }
    public Seller Seller { get; set; }
    /// <summary>Değerlendiren kullanıcı ID</summary>
    public Guid UserId { get; set; }
    /// <summary>Sipariş ID</summary>
    public Guid? OrderId { get; set; }
    /// <summary>Genel değerlendirme (1-5)</summary>
    public int OverallRating { get; set; }
    /// <summary>İletişim değerlendirmesi (1-5)</summary>
    public int CommunicationRating { get; set; }
    /// <summary>Kargo hızı değerlendirmesi (1-5)</summary>
    public int ShippingSpeedRating { get; set; }
    /// <summary>Ürün kalitesi değerlendirmesi (1-5)</summary>
    public int ProductQualityRating { get; set; }
    /// <summary>Başlık</summary>
    public string Title { get; set; }
    /// <summary>Yorum</summary>
    public string Comment { get; set; }
    /// <summary>Doğrulanmış alım mı?</summary>
    public bool IsVerifiedPurchase { get; set; }
    /// <summary>Onaylandı mı?</summary>
    public bool IsApproved { get; set; }
    /// <summary>Onay tarihi</summary>
    public DateTime? ApprovedAt { get; set; }
    /// <summary>Faydalı bulma sayısı</summary>
    public int HelpfulCount { get; set; }
    /// <summary>Faydalı bulmama sayısı</summary>
    public int NotHelpfulCount { get; set; }
    /// <summary>Satıcı yanıtı</summary>
    public string SellerResponse { get; set; }
    /// <summary>Satıcı yanıt tarihi</summary>
    public DateTime? SellerRespondedAt { get; set; }
    
    public SellerReview()
    {
        IsApproved = false;
        IsVerifiedPurchase = false;
    }
}
