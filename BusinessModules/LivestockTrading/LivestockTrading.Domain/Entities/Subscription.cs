using System;
using System.Collections.Generic;

namespace LivestockTrading.Domain.Entities;

/// <summary>
/// Abonelik plan tanımları (Free, Basic, Pro, Business)
/// App Store / Google Play IAP ürün tanımlarıyla eşleşir
/// </summary>
public class SubscriptionPlan : BaseEntity
{
    /// <summary>Plan adı</summary>
    public string Name { get; set; }
    /// <summary>Plan adı çevirileri JSON</summary>
    public string NameTranslations { get; set; }
    /// <summary>Açıklama</summary>
    public string Description { get; set; }
    /// <summary>Açıklama çevirileri JSON</summary>
    public string DescriptionTranslations { get; set; }
    /// <summary>Hedef kitle (Seller, Transporter)</summary>
    public SubscriptionTargetType TargetType { get; set; }
    /// <summary>Plan seviyesi</summary>
    public SubscriptionTier Tier { get; set; }
    /// <summary>Aylık fiyat (USD)</summary>
    public decimal PriceMonthly { get; set; }
    /// <summary>Yıllık fiyat (USD)</summary>
    public decimal PriceYearly { get; set; }
    /// <summary>Para birimi</summary>
    public string Currency { get; set; }
    /// <summary>Apple App Store ürün ID (aylık)</summary>
    public string AppleProductIdMonthly { get; set; }
    /// <summary>Apple App Store ürün ID (yıllık)</summary>
    public string AppleProductIdYearly { get; set; }
    /// <summary>Google Play ürün ID (aylık)</summary>
    public string GoogleProductIdMonthly { get; set; }
    /// <summary>Google Play ürün ID (yıllık)</summary>
    public string GoogleProductIdYearly { get; set; }
    /// <summary>Maksimum aktif ilan sayısı (0 = sınırsız)</summary>
    public int MaxActiveListings { get; set; }
    /// <summary>İlan başına maksimum fotoğraf sayısı</summary>
    public int MaxPhotosPerListing { get; set; }
    /// <summary>Aylık boost kredisi</summary>
    public int MonthlyBoostCredits { get; set; }
    /// <summary>Detaylı analitik erişimi var mı?</summary>
    public bool HasDetailedAnalytics { get; set; }
    /// <summary>Öncelikli destek var mı?</summary>
    public bool HasPrioritySupport { get; set; }
    /// <summary>Öne çıkan satıcı badge'i var mı?</summary>
    public bool HasFeaturedBadge { get; set; }
    /// <summary>Sıralama</summary>
    public int SortOrder { get; set; }
    /// <summary>Aktif mi?</summary>
    public bool IsActive { get; set; }

    public ICollection<SellerSubscription> SellerSubscriptions { get; set; }

    public SubscriptionPlan()
    {
        SellerSubscriptions = new HashSet<SellerSubscription>();
        IsActive = true;
        Currency = "USD";
        Tier = SubscriptionTier.Free;
        TargetType = SubscriptionTargetType.Seller;
    }
}

public enum SubscriptionTargetType
{
    Seller = 0,
    Transporter = 1
}

public enum SubscriptionTier
{
    Free = 0,
    Basic = 1,
    Pro = 2,
    Business = 3
}

/// <summary>
/// Satıcının aktif abonelik kaydı
/// </summary>
public class SellerSubscription : BaseEntity
{
    /// <summary>Satıcı ID</summary>
    public Guid SellerId { get; set; }
    public Seller Seller { get; set; }
    /// <summary>Abonelik planı ID</summary>
    public Guid SubscriptionPlanId { get; set; }
    public SubscriptionPlan SubscriptionPlan { get; set; }
    /// <summary>Abonelik durumu</summary>
    public SubscriptionStatus Status { get; set; }
    /// <summary>Ödeme periyodu</summary>
    public SubscriptionPeriod Period { get; set; }
    /// <summary>Satın alma platformu</summary>
    public SubscriptionPlatform Platform { get; set; }
    /// <summary>Store'daki orijinal işlem ID</summary>
    public string OriginalTransactionId { get; set; }
    /// <summary>Başlangıç tarihi</summary>
    public DateTime StartedAt { get; set; }
    /// <summary>Bitiş tarihi</summary>
    public DateTime ExpiresAt { get; set; }
    /// <summary>İptal tarihi</summary>
    public DateTime? CancelledAt { get; set; }
    /// <summary>Otomatik yenileme aktif mi?</summary>
    public bool AutoRenew { get; set; }
    /// <summary>Son doğrulama tarihi</summary>
    public DateTime? LastValidatedAt { get; set; }

    public SellerSubscription()
    {
        Status = SubscriptionStatus.Active;
        AutoRenew = true;
        StartedAt = DateTime.UtcNow;
    }
}

public enum SubscriptionStatus
{
    Active = 0,
    Expired = 1,
    Cancelled = 2,
    GracePeriod = 3
}

public enum SubscriptionPeriod
{
    Monthly = 0,
    Yearly = 1
}

public enum SubscriptionPlatform
{
    Apple = 0,
    Google = 1,
    Web = 2
}

/// <summary>
/// In-App Purchase işlem kaydı (Apple/Google receipt doğrulaması)
/// </summary>
public class IAPTransaction : BaseEntity
{
    /// <summary>Kullanıcı ID</summary>
    public Guid UserId { get; set; }
    /// <summary>Abonelik ID (abonelik işlemi ise)</summary>
    public Guid? SellerSubscriptionId { get; set; }
    public SellerSubscription SellerSubscription { get; set; }
    /// <summary>Boost ID (boost işlemi ise)</summary>
    public Guid? ProductBoostId { get; set; }
    /// <summary>İşlem tipi</summary>
    public IAPTransactionType TransactionType { get; set; }
    /// <summary>Platform</summary>
    public SubscriptionPlatform Platform { get; set; }
    /// <summary>Store işlem ID</summary>
    public string StoreTransactionId { get; set; }
    /// <summary>Store receipt verisi</summary>
    public string Receipt { get; set; }
    /// <summary>Tutar</summary>
    public decimal Amount { get; set; }
    /// <summary>Para birimi</summary>
    public string Currency { get; set; }
    /// <summary>Durum</summary>
    public IAPTransactionStatus Status { get; set; }
    /// <summary>Doğrulama tarihi</summary>
    public DateTime? ValidatedAt { get; set; }
    /// <summary>İade tarihi</summary>
    public DateTime? RefundedAt { get; set; }
    /// <summary>Store'dan gelen ham yanıt JSON</summary>
    public string RawResponse { get; set; }

    public IAPTransaction()
    {
        Status = IAPTransactionStatus.Pending;
    }
}

public enum IAPTransactionType
{
    Subscription = 0,
    Boost = 1
}

public enum IAPTransactionStatus
{
    Pending = 0,
    Validated = 1,
    Failed = 2,
    Refunded = 3
}
