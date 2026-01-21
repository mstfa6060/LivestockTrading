using System;

namespace LivestockTrading.Domain.Entities;

/// <summary>
/// Sipariş kalemi
/// </summary>
public class OrderItem : BaseEntity
{
    public Guid OrderId { get; set; }
    public Order Order { get; set; }
    public Guid ProductId { get; set; }
    public Product Product { get; set; }
    public Guid? ProductVariantId { get; set; }
    public ProductVariant ProductVariant { get; set; }
    
    /// <summary>Ürün adı (sipariş anındaki)</summary>
    public string ProductName { get; set; }
    /// <summary>Ürün SKU (sipariş anındaki)</summary>
    public string ProductSku { get; set; }
    /// <summary>Varyant adı (sipariş anındaki)</summary>
    public string VariantName { get; set; }
    /// <summary>Miktar</summary>
    public int Quantity { get; set; }
    /// <summary>Birim fiyat</summary>
    public decimal UnitPrice { get; set; }
    /// <summary>Toplam fiyat</summary>
    public decimal TotalPrice { get; set; }
    /// <summary>Vergi tutarı</summary>
    public decimal TaxAmount { get; set; }
    /// <summary>Vergi oranı</summary>
    public decimal TaxRate { get; set; }
    /// <summary>Notlar</summary>
    public string Notes { get; set; }
}

/// <summary>
/// Sipariş durum geçmişi
/// </summary>
public class OrderStatusHistory : BaseEntity
{
    public Guid OrderId { get; set; }
    public Order Order { get; set; }
    /// <summary>Önceki durum</summary>
    public OrderStatus FromStatus { get; set; }
    /// <summary>Yeni durum</summary>
    public OrderStatus ToStatus { get; set; }
    /// <summary>Notlar</summary>
    public string Notes { get; set; }
    /// <summary>Değiştiren kullanıcı ID</summary>
    public Guid? ChangedByUserId { get; set; }
    /// <summary>Değişiklik tarihi</summary>
    public DateTime ChangedAt { get; set; }
    
    public OrderStatusHistory()
    {
        ChangedAt = DateTime.UtcNow;
    }
}

/// <summary>
/// Sipariş ödeme işlemleri
/// </summary>
public class OrderPayment : BaseEntity
{
    public Guid OrderId { get; set; }
    public Order Order { get; set; }
    /// <summary>Tutar</summary>
    public decimal Amount { get; set; }
    /// <summary>Para birimi</summary>
    public string Currency { get; set; }
    /// <summary>Ödeme yöntemi</summary>
    public string PaymentMethod { get; set; }
    /// <summary>Ödeme gateway'i</summary>
    public string PaymentGateway { get; set; }
    /// <summary>İşlem ID</summary>
    public string TransactionId { get; set; }
    /// <summary>Gateway yanıtı JSON</summary>
    public string GatewayResponse { get; set; }
    /// <summary>Ödeme durumu</summary>
    public PaymentStatus Status { get; set; }
    /// <summary>Ödeme tarihi</summary>
    public DateTime? PaidAt { get; set; }
    /// <summary>İade tarihi</summary>
    public DateTime? RefundedAt { get; set; }
    /// <summary>Notlar</summary>
    public string Notes { get; set; }
}
