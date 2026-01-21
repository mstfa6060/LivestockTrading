using System;
using System.Collections.Generic;

namespace LivestockTrading.Domain.Entities;

/// <summary>
/// Alıcı-Satıcı Anlaşması
/// Platform dışında gerçekleşen ticari anlaşma kaydı
/// </summary>
public class Deal : BaseEntity
{
    /// <summary>Anlaşma numarası</summary>
    public string DealNumber { get; set; }
    
    /// <summary>Ürün ID</summary>
    public Guid ProductId { get; set; }
    public Product Product { get; set; }
    
    /// <summary>Satıcı ID</summary>
    public Guid SellerId { get; set; }
    public Seller Seller { get; set; }
    
    /// <summary>Alıcı ID</summary>
    public Guid BuyerId { get; set; }
    
    /// <summary>Anlaşılan fiyat</summary>
    public decimal AgreedPrice { get; set; }
    
    /// <summary>Para birimi</summary>
    public string Currency { get; set; }
    
    /// <summary>Miktar</summary>
    public int Quantity { get; set; }
    
    /// <summary>Anlaşma durumu</summary>
    public DealStatus Status { get; set; }
    
    /// <summary>Anlaşma tarihi</summary>
    public DateTime DealDate { get; set; }
    
    /// <summary>Teslim şekli (El değişimi, Nakliye vb.)</summary>
    public DeliveryMethod DeliveryMethod { get; set; }
    
    /// <summary>Teslim tarihi</summary>
    public DateTime? DeliveryDate { get; set; }
    
    /// <summary>Alıcı notları</summary>
    public string BuyerNotes { get; set; }
    
    /// <summary>Satıcı notları</summary>
    public string SellerNotes { get; set; }
    
    /// <summary>Sözleşme belgeleri (PDF)</summary>
    public string ContractDocuments { get; set; }
    
    /// <summary>Nakliye talebi oluşturuldu mu?</summary>
    public bool TransportRequestCreated { get; set; }
    
    /// <summary>Nakliye talebi ID (eğer oluşturulduysa)</summary>
    public Guid? TransportRequestId { get; set; }
    public TransportRequest TransportRequest { get; set; }
    
    /// <summary>Anlaşma tamamlandı mı?</summary>
    public bool IsCompleted { get; set; }
    
    /// <summary>Tamamlanma tarihi</summary>
    public DateTime? CompletedAt { get; set; }
    
    /// <summary>İptal edildi mi?</summary>
    public bool IsCancelled { get; set; }
    
    /// <summary>İptal tarihi</summary>
    public DateTime? CancelledAt { get; set; }
    
    /// <summary>İptal nedeni</summary>
    public string CancellationReason { get; set; }
    
    public Deal()
    {
        DealDate = DateTime.UtcNow;
        Status = DealStatus.Agreed;
        DeliveryMethod = DeliveryMethod.ToBeDecided;
        TransportRequestCreated = false;
        IsCompleted = false;
        IsCancelled = false;
    }
}

public enum DealStatus
{
    /// <summary>Anlaşıldı</summary>
    Agreed = 0,
    /// <summary>Ödeme bekleniyor</summary>
    AwaitingPayment = 1,
    /// <summary>Ödeme yapıldı</summary>
    Paid = 2,
    /// <summary>Hazırlanıyor</summary>
    Preparing = 3,
    /// <summary>Teslimatta</summary>
    InDelivery = 4,
    /// <summary>Teslim edildi</summary>
    Delivered = 5,
    /// <summary>Tamamlandı</summary>
    Completed = 6,
    /// <summary>İptal</summary>
    Cancelled = 7
}

public enum DeliveryMethod
{
    /// <summary>Henüz belirlenmedi</summary>
    ToBeDecided = 0,
    /// <summary>Alıcı kendisi alacak</summary>
    SelfPickup = 1,
    /// <summary>Satıcı teslim edecek</summary>
    SellerDelivery = 2,
    /// <summary>Nakliyeci ile</summary>
    ThirdPartyTransport = 3,
    /// <summary>Kargo ile</summary>
    Courier = 4
}
