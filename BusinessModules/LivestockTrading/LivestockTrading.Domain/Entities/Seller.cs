using System;
using System.Collections.Generic;

namespace LivestockTrading.Domain.Entities;

/// <summary>
/// Satıcı/Tedarikçi
/// Çiftçiler, tarım şirketleri, bayiler
/// </summary>
public class Seller : BaseEntity
{
    /// <summary>Kullanıcı ID (IAM'den)</summary>
    public Guid UserId { get; set; }
    /// <summary>İşletme adı</summary>
    public string BusinessName { get; set; }
    /// <summary>İşletme tipi (Bireysel Çiftçi, Şirket, Bayi, Kooperatif)</summary>
    public string BusinessType { get; set; }
    /// <summary>Vergi numarası</summary>
    public string TaxNumber { get; set; }
    /// <summary>Sicil numarası</summary>
    public string RegistrationNumber { get; set; }
    /// <summary>Açıklama</summary>
    public string Description { get; set; }
    /// <summary>Logo URL (FileProvider'dan)</summary>
    public string LogoUrl { get; set; }
    /// <summary>Banner URL (FileProvider'dan)</summary>
    public string BannerUrl { get; set; }
    /// <summary>E-posta</summary>
    public string Email { get; set; }
    /// <summary>Telefon</summary>
    public string Phone { get; set; }
    /// <summary>Web sitesi</summary>
    public string Website { get; set; }
    /// <summary>Doğrulanmış mı?</summary>
    public bool IsVerified { get; set; }
    /// <summary>Doğrulanma tarihi</summary>
    public DateTime? VerifiedAt { get; set; }
    /// <summary>Aktif mi?</summary>
    public bool IsActive { get; set; }
    /// <summary>Satıcı durumu</summary>
    public SellerStatus Status { get; set; }
    /// <summary>Ortalama değerlendirme</summary>
    public decimal? AverageRating { get; set; }
    /// <summary>Toplam değerlendirme sayısı</summary>
    public int TotalReviews { get; set; }
    /// <summary>Toplam satış sayısı</summary>
    public int TotalSales { get; set; }
    /// <summary>Toplam gelir</summary>
    public decimal TotalRevenue { get; set; }
    /// <summary>Çalışma saatleri JSON</summary>
    public string BusinessHours { get; set; }
    /// <summary>Kabul edilen ödeme yöntemleri JSON</summary>
    public string AcceptedPaymentMethods { get; set; }
    /// <summary>İade politikası</summary>
    public string ReturnPolicy { get; set; }
    /// <summary>Kargo politikası</summary>
    public string ShippingPolicy { get; set; }
    /// <summary>Sosyal medya linkleri JSON</summary>
    public string SocialMediaLinks { get; set; }
    /// <summary>Aktif abonelik ID</summary>
    public Guid? ActiveSubscriptionId { get; set; }
    public SellerSubscription ActiveSubscription { get; set; }

    public ICollection<Product> Products { get; set; }
    public ICollection<Farm> Farms { get; set; }
    public ICollection<SellerReview> Reviews { get; set; }

    public Seller()
    {
        Products = new HashSet<Product>();
        Farms = new HashSet<Farm>();
        Reviews = new HashSet<SellerReview>();
        IsActive = true;
        Status = SellerStatus.PendingVerification;
    }
}

public enum SellerStatus
{
    PendingVerification = 0,
    Active = 1,
    Suspended = 2,
    Banned = 3,
    Inactive = 4
}
