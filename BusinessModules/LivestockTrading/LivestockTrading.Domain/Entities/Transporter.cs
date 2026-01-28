using System;
using System.Collections.Generic;

namespace LivestockTrading.Domain.Entities;

/// <summary>
/// Nakliyeci profili
/// Taşıma hizmeti veren kişi/firma
/// </summary>
public class Transporter : BaseEntity
{
    /// <summary>Kullanıcı ID (IAM)</summary>
    public Guid UserId { get; set; }
    
    /// <summary>Firma adı</summary>
    public string CompanyName { get; set; }
    
    /// <summary>İletişim kişisi</summary>
    public string ContactPerson { get; set; }
    
    /// <summary>E-posta</summary>
    public string Email { get; set; }
    
    /// <summary>Telefon</summary>
    public string Phone { get; set; }
    
    /// <summary>Adres</summary>
    public string Address { get; set; }
    
    /// <summary>Şehir</summary>
    public string City { get; set; }
    
    /// <summary>Ülke kodu</summary>
    public string CountryCode { get; set; }
    
    /// <summary>Logo URL (FileProvider)</summary>
    public string LogoUrl { get; set; }
    
    /// <summary>Açıklama</summary>
    public string Description { get; set; }
    
    /// <summary>Ruhsat numarası</summary>
    public string LicenseNumber { get; set; }
    
    /// <summary>Vergi numarası</summary>
    public string TaxNumber { get; set; }
    
    /// <summary>Sigorta bilgileri</summary>
    public string InsuranceInfo { get; set; }
    
    /// <summary>Araç filosu bilgisi (JSON)</summary>
    public string FleetInfo { get; set; }
    
    /// <summary>Hizmet verilen bölgeler (JSON: ["TR","GR","BG"])</summary>
    public string ServiceRegions { get; set; }
    
    /// <summary>Uzmanlaşma alanları (JSON: ["Canlı hayvan","Soğuk zincir"])</summary>
    public string Specializations { get; set; }
    
    /// <summary>Doğrulanmış mı?</summary>
    public bool IsVerified { get; set; }
    
    /// <summary>Doğrulanma tarihi</summary>
    public DateTime? VerifiedAt { get; set; }
    
    /// <summary>Aktif mi?</summary>
    public bool IsActive { get; set; }

    /// <summary>Nakliyeci durumu</summary>
    public TransporterStatus Status { get; set; }

    /// <summary>Ortalama değerlendirme</summary>
    public decimal? AverageRating { get; set; }
    
    /// <summary>Toplam taşıma sayısı</summary>
    public int TotalTransports { get; set; }
    
    /// <summary>Başarıyla tamamlanan taşıma sayısı</summary>
    public int CompletedTransports { get; set; }
    
    /// <summary>Web sitesi</summary>
    public string Website { get; set; }
    
    /// <summary>Sertifikalar (JSON)</summary>
    public string Certifications { get; set; }
    
    /// <summary>Belge URL'leri (JSON)</summary>
    public string DocumentUrls { get; set; }
    
    /// <summary>Atanan taşıma talepleri</summary>
    public ICollection<TransportRequest> AssignedTransports { get; set; }
    
    /// <summary>Verilen teklifler</summary>
    public ICollection<TransportOffer> TransportOffers { get; set; }
    
    /// <summary>Değerlendirmeler</summary>
    public ICollection<TransporterReview> Reviews { get; set; }
    
    public Transporter()
    {
        AssignedTransports = new HashSet<TransportRequest>();
        TransportOffers = new HashSet<TransportOffer>();
        Reviews = new HashSet<TransporterReview>();
        IsActive = true;
        IsVerified = false;
        Status = TransporterStatus.PendingVerification;
    }
}

public enum TransporterStatus
{
    PendingVerification = 0,
    Active = 1,
    Suspended = 2,
    Banned = 3,
    Inactive = 4
}

/// <summary>
/// Nakliyeci değerlendirmesi
/// </summary>
public class TransporterReview : BaseEntity
{
    public Guid TransporterId { get; set; }
    public Transporter Transporter { get; set; }
    
    /// <summary>Değerlendiren kullanıcı ID</summary>
    public Guid UserId { get; set; }
    
    /// <summary>Nakliye talebi ID</summary>
    public Guid TransportRequestId { get; set; }
    public TransportRequest TransportRequest { get; set; }
    
    /// <summary>Genel değerlendirme (1-5)</summary>
    public int OverallRating { get; set; }
    
    /// <summary>Zamanında teslimat (1-5)</summary>
    public int TimelinessRating { get; set; }
    
    /// <summary>İletişim (1-5)</summary>
    public int CommunicationRating { get; set; }
    
    /// <summary>Ürün güvenliği (1-5)</summary>
    public int CarefulHandlingRating { get; set; }
    
    /// <summary>Profesyonellik (1-5)</summary>
    public int ProfessionalismRating { get; set; }
    
    /// <summary>Yorum</summary>
    public string Comment { get; set; }
    
    /// <summary>Onaylandı mı?</summary>
    public bool IsApproved { get; set; }
    
    public TransporterReview()
    {
        IsApproved = false;
    }
}
