using System;
using System.Collections.Generic;

namespace LivestockTrading.Domain.Entities;

/// <summary>
/// Sipariş bilgileri
/// </summary>
public class Order : BaseEntity
{
    /// <summary>Sipariş numarası (Benzersiz)</summary>
    public string OrderNumber { get; set; }
    /// <summary>Alıcı ID (IAM kullanıcı ID)</summary>
    public Guid BuyerId { get; set; }
    /// <summary>Satıcı ID</summary>
    public Guid SellerId { get; set; }
    public Seller Seller { get; set; }
    
    /// <summary>Ara toplam</summary>
    public decimal SubTotal { get; set; }
    /// <summary>Kargo ücreti</summary>
    public decimal ShippingCost { get; set; }
    /// <summary>Vergi tutarı</summary>
    public decimal TaxAmount { get; set; }
    /// <summary>İndirim tutarı</summary>
    public decimal DiscountAmount { get; set; }
    /// <summary>Toplam tutar</summary>
    public decimal TotalAmount { get; set; }
    /// <summary>Para birimi</summary>
    public string Currency { get; set; }
    
    /// <summary>Kargo adresi ID</summary>
    public Guid ShippingAddressId { get; set; }
    public Location ShippingAddress { get; set; }
    /// <summary>Fatura adresi ID</summary>
    public Guid BillingAddressId { get; set; }
    public Location BillingAddress { get; set; }
    
    /// <summary>Sipariş durumu</summary>
    public OrderStatus Status { get; set; }
    /// <summary>Ödeme durumu</summary>
    public PaymentStatus PaymentStatus { get; set; }
    /// <summary>Kargo durumu</summary>
    public ShippingStatus ShippingStatus { get; set; }
    
    /// <summary>Sipariş tarihi</summary>
    public DateTime OrderDate { get; set; }
    /// <summary>Ödeme tarihi</summary>
    public DateTime? PaidAt { get; set; }
    /// <summary>Kargoya verilme tarihi</summary>
    public DateTime? ShippedAt { get; set; }
    /// <summary>Teslim tarihi</summary>
    public DateTime? DeliveredAt { get; set; }
    /// <summary>İptal tarihi</summary>
    public DateTime? CancelledAt { get; set; }
    
    /// <summary>Müşteri notları</summary>
    public string CustomerNotes { get; set; }
    /// <summary>Satıcı notları</summary>
    public string SellerNotes { get; set; }
    /// <summary>Admin notları</summary>
    public string AdminNotes { get; set; }
    
    /// <summary>Kargo takip numarası</summary>
    public string TrackingNumber { get; set; }
    /// <summary>Kargo firması</summary>
    public string ShippingCarrier { get; set; }
    
    /// <summary>Ödeme yöntemi</summary>
    public string PaymentMethod { get; set; }
    /// <summary>Ödeme işlem ID</summary>
    public string PaymentTransactionId { get; set; }
    
    public ICollection<OrderItem> Items { get; set; }
    public ICollection<OrderStatusHistory> StatusHistory { get; set; }
    public ICollection<OrderPayment> Payments { get; set; }
    
    public Order()
    {
        Items = new HashSet<OrderItem>();
        StatusHistory = new HashSet<OrderStatusHistory>();
        Payments = new HashSet<OrderPayment>();
        OrderDate = DateTime.UtcNow;
        Status = OrderStatus.Pending;
        PaymentStatus = PaymentStatus.Pending;
        ShippingStatus = ShippingStatus.NotShipped;
        Currency = "USD";
    }
}

public enum OrderStatus
{
    Pending = 0,
    Confirmed = 1,
    Processing = 2,
    Shipped = 3,
    Delivered = 4,
    Cancelled = 5,
    Refunded = 6,
    Failed = 7
}

public enum PaymentStatus
{
    Pending = 0,
    Paid = 1,
    Failed = 2,
    Refunded = 3,
    PartiallyRefunded = 4
}

public enum ShippingStatus
{
    NotShipped = 0,
    Processing = 1,
    Shipped = 2,
    InTransit = 3,
    OutForDelivery = 4,
    Delivered = 5,
    Failed = 6,
    Returned = 7
}
