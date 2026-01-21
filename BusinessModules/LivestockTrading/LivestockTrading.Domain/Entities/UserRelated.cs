using System;
using System.Collections.Generic;

namespace LivestockTrading.Domain.Entities;

/// <summary>
/// Favori ürünler (istek listesi)
/// </summary>
public class FavoriteProduct : BaseEntity
{
    /// <summary>Kullanıcı ID</summary>
    public Guid UserId { get; set; }
    public Guid ProductId { get; set; }
    public Product Product { get; set; }
    /// <summary>Eklenme tarihi</summary>
    public DateTime AddedAt { get; set; }
    
    public FavoriteProduct()
    {
        AddedAt = DateTime.UtcNow;
    }
}

/// <summary>
/// Kullanıcı bildirimleri
/// </summary>
public class Notification : BaseEntity
{
    /// <summary>Kullanıcı ID</summary>
    public Guid UserId { get; set; }
    /// <summary>Başlık</summary>
    public string Title { get; set; }
    /// <summary>Mesaj</summary>
    public string Message { get; set; }
    /// <summary>Bildirim tipi</summary>
    public NotificationType Type { get; set; }
    /// <summary>Aksiyon URL</summary>
    public string ActionUrl { get; set; }
    /// <summary>Aksiyon verisi JSON</summary>
    public string ActionData { get; set; }
    /// <summary>Okundu mu?</summary>
    public bool IsRead { get; set; }
    /// <summary>Okunma tarihi</summary>
    public DateTime? ReadAt { get; set; }
    /// <summary>Gönderilme tarihi</summary>
    public DateTime SentAt { get; set; }
    
    public Notification()
    {
        IsRead = false;
        SentAt = DateTime.UtcNow;
    }
}

public enum NotificationType
{
    OrderPlaced = 0,
    OrderShipped = 1,
    OrderDelivered = 2,
    OrderCancelled = 3,
    PaymentReceived = 4,
    PaymentFailed = 5,
    NewMessage = 6,
    ProductBackInStock = 7,
    PriceDropAlert = 8,
    NewReview = 9,
    SellerVerified = 10,
    ProductApproved = 11,
    ProductRejected = 12,
    System = 99
}

/// <summary>
/// Kullanıcı arama geçmişi
/// </summary>
public class SearchHistory : BaseEntity
{
    /// <summary>Kullanıcı ID</summary>
    public Guid UserId { get; set; }
    /// <summary>Arama sorgusu</summary>
    public string SearchQuery { get; set; }
    /// <summary>Uygulanan filtreler JSON</summary>
    public string Filters { get; set; }
    /// <summary>Sonuç sayısı</summary>
    public int ResultsCount { get; set; }
    /// <summary>Arama tarihi</summary>
    public DateTime SearchedAt { get; set; }
    
    public SearchHistory()
    {
        SearchedAt = DateTime.UtcNow;
    }
}

/// <summary>
/// Ürün görüntüleme geçmişi
/// </summary>
public class ProductViewHistory : BaseEntity
{
    /// <summary>Kullanıcı ID</summary>
    public Guid UserId { get; set; }
    public Guid ProductId { get; set; }
    public Product Product { get; set; }
    /// <summary>Görüntüleme tarihi</summary>
    public DateTime ViewedAt { get; set; }
    /// <summary>Görüntüleme kaynağı (Arama, Kategori, Öneri, Doğrudan)</summary>
    public string ViewSource { get; set; }
    
    public ProductViewHistory()
    {
        ViewedAt = DateTime.UtcNow;
    }
}
