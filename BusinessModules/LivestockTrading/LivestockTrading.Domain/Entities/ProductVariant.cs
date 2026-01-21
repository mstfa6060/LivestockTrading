using System;
using System.Collections.Generic;

namespace LivestockTrading.Domain.Entities;

/// <summary>
/// Ürün varyantları (boyut, renk, ambalaj vb.)
/// Örnek: Aynı gübre 5kg, 10kg, 25kg paketlerle
/// </summary>
public class ProductVariant : BaseEntity
{
    public Guid ProductId { get; set; }
    public Product Product { get; set; }
    
    /// <summary>Varyant adı</summary>
    public string Name { get; set; }
    /// <summary>Stok Kodu (SKU)</summary>
    public string SKU { get; set; }
    /// <summary>Fiyat (ana ürün fiyatını geçersiz kılabilir)</summary>
    public decimal? Price { get; set; }
    /// <summary>İndirimli fiyat</summary>
    public decimal? DiscountedPrice { get; set; }
    /// <summary>Stok miktarı</summary>
    public int StockQuantity { get; set; }
    /// <summary>Stokta var mı?</summary>
    public bool IsInStock { get; set; }
    /// <summary>Varyanta özel özellikler JSON</summary>
    public string Attributes { get; set; }
    /// <summary>Varyant görseli URL (FileProvider'dan)</summary>
    public string ImageUrl { get; set; }
    /// <summary>Aktif mi?</summary>
    public bool IsActive { get; set; }
    /// <summary>Sıralama</summary>
    public int SortOrder { get; set; }
    
    public ICollection<CartItem> CartItems { get; set; }
    public ICollection<OrderItem> OrderItems { get; set; }
    
    public ProductVariant()
    {
        CartItems = new HashSet<CartItem>();
        OrderItems = new HashSet<OrderItem>();
        IsActive = true;
        IsInStock = true;
    }
}
