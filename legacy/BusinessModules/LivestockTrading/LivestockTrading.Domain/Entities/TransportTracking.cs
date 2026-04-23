using System;

namespace LivestockTrading.Domain.Entities;

/// <summary>
/// Nakliye takip kaydı
/// Taşıma süresince konum ve durum güncellemeleri
/// </summary>
public class TransportTracking : BaseEntity
{
    /// <summary>Nakliye talebi ID</summary>
    public Guid TransportRequestId { get; set; }
    public TransportRequest TransportRequest { get; set; }

    /// <summary>Enlem</summary>
    public double? Latitude { get; set; }

    /// <summary>Boylam</summary>
    public double? Longitude { get; set; }

    /// <summary>Konum açıklaması</summary>
    public string LocationDescription { get; set; }

    /// <summary>Durum</summary>
    public TrackingStatus Status { get; set; }

    /// <summary>Durum açıklaması</summary>
    public string StatusDescription { get; set; }

    /// <summary>Kayıt zamanı</summary>
    public DateTime RecordedAt { get; set; }

    /// <summary>Notlar</summary>
    public string Notes { get; set; }

    /// <summary>Fotoğraf URL'leri (JSON)</summary>
    public string PhotoUrls { get; set; }

    public TransportTracking()
    {
        RecordedAt = DateTime.UtcNow;
        Status = TrackingStatus.Update;
    }
}

public enum TrackingStatus
{
    /// <summary>Genel güncelleme</summary>
    Update = 0,
    /// <summary>Toplama noktasına ulaşıldı</summary>
    ArrivedAtPickup = 1,
    /// <summary>Yükleme yapıldı</summary>
    Loaded = 2,
    /// <summary>Yola çıkıldı</summary>
    Departed = 3,
    /// <summary>Mola</summary>
    RestStop = 4,
    /// <summary>Sınır geçişi</summary>
    BorderCrossing = 5,
    /// <summary>Varış noktasına ulaşıldı</summary>
    ArrivedAtDestination = 6,
    /// <summary>Boşaltma yapıldı</summary>
    Unloaded = 7,
    /// <summary>Teslim edildi</summary>
    Delivered = 8,
    /// <summary>Gecikme</summary>
    Delay = 9,
    /// <summary>Sorun</summary>
    Issue = 10
}
