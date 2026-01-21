using System;
using System.Collections.Generic;

namespace LivestockTrading.Domain.Entities;

/// <summary>
/// Alışveriş sepeti
/// </summary>
public class Cart : BaseEntity
{
    /// <summary>Kullanıcı ID (IAM)</summary>
    public Guid UserId { get; set; }
    /// <summary>Oturum ID (misafir kullanıcılar için)</summary>
    public string SessionId { get; set; }
    /// <summary>Ara toplam</summary>
    public decimal SubTotal { get; set; }
    /// <summary>Toplam tutar</summary>
    public decimal TotalAmount { get; set; }
    /// <summary>Para birimi</summary>
    public string Currency { get; set; }
    /// <summary>Son aktivite tarihi</summary>
    public DateTime? LastActivityAt { get; set; }
    /// <summary>Son kullanma tarihi</summary>
    public DateTime? ExpiresAt { get; set; }
    
    public ICollection<CartItem> Items { get; set; }
    
    public Cart()
    {
        Items = new HashSet<CartItem>();
        Currency = "USD";
        LastActivityAt = DateTime.UtcNow;
        ExpiresAt = DateTime.UtcNow.AddDays(30);
    }
}

/// <summary>
/// Sepet kalemi
/// </summary>
public class CartItem : BaseEntity
{
    public Guid CartId { get; set; }
    public Cart Cart { get; set; }
    public Guid ProductId { get; set; }
    public Product Product { get; set; }
    public Guid? ProductVariantId { get; set; }
    public ProductVariant ProductVariant { get; set; }
    /// <summary>Miktar</summary>
    public int Quantity { get; set; }
    /// <summary>Birim fiyat</summary>
    public decimal UnitPrice { get; set; }
    /// <summary>Toplam fiyat</summary>
    public decimal TotalPrice { get; set; }
    /// <summary>Eklenme tarihi</summary>
    public DateTime AddedAt { get; set; }
    
    public CartItem()
    {
        AddedAt = DateTime.UtcNow;
        Quantity = 1;
    }
}
