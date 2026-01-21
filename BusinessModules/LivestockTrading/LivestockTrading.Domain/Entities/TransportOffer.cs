using System;

namespace LivestockTrading.Domain.Entities;

/// <summary>
/// Nakliyeci teklifi
/// Nakliyeciler havuzdaki taleplere teklif verir
/// </summary>
public class TransportOffer : BaseEntity
{
    /// <summary>Nakliye talebi ID</summary>
    public Guid TransportRequestId { get; set; }
    public TransportRequest TransportRequest { get; set; }
    
    /// <summary>Nakliyeci ID</summary>
    public Guid TransporterId { get; set; }
    public Transporter Transporter { get; set; }
    
    /// <summary>Teklif edilen fiyat</summary>
    public decimal OfferedPrice { get; set; }
    
    /// <summary>Para birimi</summary>
    public string Currency { get; set; }
    
    /// <summary>Tahmini toplama tarihi</summary>
    public DateTime? EstimatedPickupDate { get; set; }
    
    /// <summary>Tahmini teslim tarihi</summary>
    public DateTime? EstimatedDeliveryDate { get; set; }
    
    /// <summary>Tahmini süre (gün)</summary>
    public int? EstimatedDurationDays { get; set; }
    
    /// <summary>Kullanılacak araç tipi</summary>
    public string VehicleType { get; set; }
    
    /// <summary>Sigorta dahil mi?</summary>
    public bool InsuranceIncluded { get; set; }
    
    /// <summary>Sigorta tutarı</summary>
    public decimal? InsuranceAmount { get; set; }
    
    /// <summary>Ek hizmetler (JSON: ["Soğuk zincir","Canlı hayvan bakımı"])</summary>
    public string AdditionalServices { get; set; }
    
    /// <summary>Teklif mesajı</summary>
    public string Message { get; set; }
    
    /// <summary>Teklif durumu</summary>
    public TransportOfferStatus Status { get; set; }
    
    /// <summary>Teklif tarihi</summary>
    public DateTime OfferDate { get; set; }
    
    /// <summary>Son geçerlilik tarihi</summary>
    public DateTime? ExpiryDate { get; set; }
    
    /// <summary>Yanıtlanma tarihi</summary>
    public DateTime? RespondedAt { get; set; }
    
    /// <summary>Yanıt mesajı</summary>
    public string ResponseMessage { get; set; }
    
    public TransportOffer()
    {
        Status = TransportOfferStatus.Pending;
        OfferDate = DateTime.UtcNow;
        InsuranceIncluded = false;
    }
}

public enum TransportOfferStatus
{
    /// <summary>Beklemede</summary>
    Pending = 0,
    /// <summary>Kabul edildi</summary>
    Accepted = 1,
    /// <summary>Reddedildi</summary>
    Rejected = 2,
    /// <summary>Süresi doldu</summary>
    Expired = 3,
    /// <summary>Geri çekildi</summary>
    Withdrawn = 4
}
