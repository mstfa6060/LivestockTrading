using System;
using System.Collections.Generic;

namespace LivestockTrading.Domain.Entities;

/// <summary>
/// Nakliye Talebi
/// Alıcı-satıcı anlaştıktan sonra ürünün taşınması için oluşturulur
/// </summary>
public class TransportRequest : BaseEntity
{
    /// <summary>Ürün ID (hangi ürün taşınacak)</summary>
    public Guid ProductId { get; set; }
    public Product Product { get; set; }
    
    /// <summary>Satıcı (Gönderen)</summary>
    public Guid SellerId { get; set; }
    public Seller Seller { get; set; }
    
    /// <summary>Alıcı ID</summary>
    public Guid BuyerId { get; set; }
    
    /// <summary>Anlaşılan fiyat (bilgi amaçlı)</summary>
    public decimal? AgreedPrice { get; set; }
    
    /// <summary>Para birimi</summary>
    public string Currency { get; set; }
    
    /// <summary>Başlangıç konumu ID</summary>
    public Guid PickupLocationId { get; set; }
    public Location PickupLocation { get; set; }
    
    /// <summary>Varış konumu ID</summary>
    public Guid DeliveryLocationId { get; set; }
    public Location DeliveryLocation { get; set; }
    
    /// <summary>Tahmini mesafe (km)</summary>
    public decimal? EstimatedDistanceKm { get; set; }
    
    /// <summary>Ürün ağırlığı (kg)</summary>
    public decimal? WeightKg { get; set; }
    
    /// <summary>Hacim (m³)</summary>
    public decimal? VolumeCubicMeters { get; set; }
    
    /// <summary>Özel talimatlar (Soğuk zincir, dikkat edilmesi gerekenler)</summary>
    public string SpecialInstructions { get; set; }
    
    /// <summary>Tercih edilen toplama tarihi</summary>
    public DateTime? PreferredPickupDate { get; set; }
    
    /// <summary>Tercih edilen teslim tarihi</summary>
    public DateTime? PreferredDeliveryDate { get; set; }
    
    /// <summary>Acil mi?</summary>
    public bool IsUrgent { get; set; }
    
    /// <summary>Nakliye türü</summary>
    public TransportType TransportType { get; set; }
    
    /// <summary>Durum</summary>
    public TransportRequestStatus Status { get; set; }
    
    /// <summary>Nakliye havuzunda mı?</summary>
    public bool IsInPool { get; set; }
    
    /// <summary>Havuza eklenme tarihi</summary>
    public DateTime? AddedToPoolAt { get; set; }
    
    /// <summary>Seçilen nakliyeci ID (eğer atandıysa)</summary>
    public Guid? AssignedTransporterId { get; set; }
    public Transporter AssignedTransporter { get; set; }
    
    /// <summary>Atanma tarihi</summary>
    public DateTime? AssignedAt { get; set; }
    
    /// <summary>Toplama tarihi</summary>
    public DateTime? PickedUpAt { get; set; }
    
    /// <summary>Teslim tarihi</summary>
    public DateTime? DeliveredAt { get; set; }
    
    /// <summary>İptal tarihi</summary>
    public DateTime? CancelledAt { get; set; }
    
    /// <summary>İptal nedeni</summary>
    public string CancellationReason { get; set; }
    
    /// <summary>Notlar</summary>
    public string Notes { get; set; }
    
    /// <summary>Nakliyeci teklifleri</summary>
    public ICollection<TransportOffer> TransportOffers { get; set; }
    
    /// <summary>Takip kayıtları</summary>
    public ICollection<TransportTracking> TrackingHistory { get; set; }
    
    public TransportRequest()
    {
        TransportOffers = new HashSet<TransportOffer>();
        TrackingHistory = new HashSet<TransportTracking>();
        Status = TransportRequestStatus.Pending;
        IsInPool = false;
        IsUrgent = false;
    }
}

public enum TransportType
{
    /// <summary>Kara yolu</summary>
    Road = 0,
    /// <summary>Deniz yolu</summary>
    Sea = 1,
    /// <summary>Hava yolu</summary>
    Air = 2,
    /// <summary>Demiryolu</summary>
    Rail = 3,
    /// <summary>Kombine</summary>
    Multimodal = 4
}

public enum TransportRequestStatus
{
    /// <summary>Beklemede (Havuza eklenmemiş)</summary>
    Pending = 0,
    /// <summary>Havuzda (Nakliyeciler görebilir)</summary>
    InPool = 1,
    /// <summary>Teklif alma aşamasında</summary>
    ReceivingOffers = 2,
    /// <summary>Nakliyeci atandı</summary>
    Assigned = 3,
    /// <summary>Toplandı</summary>
    PickedUp = 4,
    /// <summary>Yolda</summary>
    InTransit = 5,
    /// <summary>Teslim edildi</summary>
    Delivered = 6,
    /// <summary>İptal edildi</summary>
    Cancelled = 7,
    /// <summary>Tamamlandı</summary>
    Completed = 8
}
