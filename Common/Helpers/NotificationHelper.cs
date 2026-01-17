using Microsoft.Extensions.Configuration;

namespace Common.Helpers;

/// <summary>
/// Bildirim gönderimi için tutarlı yapı sağlayan helper class
/// Ortam bazlı (dev/prod) domain konfigürasyonu destekler
///
/// Usage (Dependency Injection):
/// services.AddSingleton<NotificationHelper>();
/// 
///  URL'ler frontend route'larıyla senkronize edildi (14 Aralık 2025)
/// </summary>
public class NotificationHelper
{
    private readonly string _baseWebUrl;
    private const string DeepLinkScheme = "animalmarket://";

    public NotificationHelper(IConfiguration configuration)
    {
        // Ortam bazlı web URL'i al
        var environment = configuration["ASPNETCORE_ENVIRONMENT"] ?? "Development";

        _baseWebUrl = environment.ToLower() switch
        {
            "production" => "https://besilik.com",
            "development" => "https://dev.besilik.com",
            _ => "https://dev.besilik.com"
        };
    }

    /// <summary>
    /// İlan detay için deep link ve web URL oluştur
    /// Normal kullanıcı: /animals/{id}
    /// Admin: /admin/listings (liste sayfası, detay sayfası yok)
    /// </summary>
    public (string deepLink, string webUrl) CreateAnimalLinks(Guid animalId, bool isAdmin = false)
    {
        //  FIX: Admin için /admin/listings sayfasına yönlendir (frontend'de /admin/animals/{id} sayfası yok)
        var path = isAdmin ? "admin/listings" : $"animals/{animalId}";
        return (
            deepLink: $"{DeepLinkScheme}{path}",
            webUrl: $"{_baseWebUrl}/{path}"
        );
    }

    /// <summary>
    /// Teklif detay için deep link ve web URL oluştur
    ///  FIX: /bids/{id} sayfası yok, /my-bids sayfasına yönlendir
    /// </summary>
    public (string deepLink, string webUrl) CreateBidLinks(Guid animalId)
    {
        // Frontend'de /bids/{id} sayfası yok, /my-bids kullanılıyor
        var path = "my-bids";
        return (
            deepLink: $"{DeepLinkScheme}{path}",
            webUrl: $"{_baseWebUrl}/{path}"
        );
    }

    /// <summary>
    /// Veteriner iş/onay detay için deep link ve web URL oluştur
    ///  FIX: /vetjobs/{id} sayfası yok, /veterinarian/my-jobs sayfasına yönlendir
    /// </summary>
    public (string deepLink, string webUrl) CreateVetJobLinks(Guid jobId)
    {
        // Frontend'de /vetjobs/{id} sayfası yok, /veterinarian/my-jobs kullanılıyor
        var path = "veterinarian/my-jobs";
        return (
            deepLink: $"{DeepLinkScheme}{path}",
            webUrl: $"{_baseWebUrl}/{path}"
        );
    }

    /// <summary>
    /// Teslimat detay için deep link ve web URL oluştur
    /// ⚠️ NOT: Frontend'de /deliveries/[id] sayfası henüz yapılmadı ama planlanmış
    /// </summary>
    public (string deepLink, string webUrl) CreateDeliveryLinks(Guid deliveryId)
    {
        var path = $"deliveries/{deliveryId}";
        return (
            deepLink: $"{DeepLinkScheme}{path}",
            webUrl: $"{_baseWebUrl}/{path}"
        );
    }

    /// <summary>
    /// Admin onay sayfası için deep link ve web URL oluştur
    ///  FIX: /admin/approvals sayfası yok, /admin/listings sayfasına yönlendir
    /// </summary>
    public (string deepLink, string webUrl) CreateAdminApprovalLinks()
    {
        // Frontend'de /admin/approvals sayfası yok, /admin/listings kullanılıyor
        var path = "admin/listings";
        return (
            deepLink: $"{DeepLinkScheme}{path}",
            webUrl: $"{_baseWebUrl}/{path}"
        );
    }

    /// <summary>
    /// Satın alımlarım sayfası için deep link ve web URL oluştur
    ///  FIX: /purchases değil /my-purchases
    /// </summary>
    public (string deepLink, string webUrl) CreatePurchasesLinks()
    {
        var path = "my-purchases";
        return (
            deepLink: $"{DeepLinkScheme}{path}",
            webUrl: $"{_baseWebUrl}/{path}"
        );
    }

    /// <summary>
    /// Satışlarım sayfası için deep link ve web URL oluştur
    ///  FIX: /sales değil /my-sales
    /// </summary>
    public (string deepLink, string webUrl) CreateSalesLinks()
    {
        var path = "my-sales";
        return (
            deepLink: $"{DeepLinkScheme}{path}",
            webUrl: $"{_baseWebUrl}/{path}"
        );
    }

    /// <summary>
    /// İlanlarım sayfası için deep link ve web URL oluştur
    ///  FIX: /mylistings değil /my-listings
    /// </summary>
    public (string deepLink, string webUrl) CreateMyListingsLinks()
    {
        var path = "my-listings";
        return (
            deepLink: $"{DeepLinkScheme}{path}",
            webUrl: $"{_baseWebUrl}/{path}"
        );
    }

    /// <summary>
    /// Push notification için standart yapı oluştur
    /// </summary>
    public object CreateNotificationPayload(
        string title,
        string body,
        string type,
        string deepLink,
        string webUrl,
        object data,
        Guid? targetUserId = null,
        bool targetAdmins = false,
        List<string>? adminEmails = null,
        string priority = "high")
    {
        return new
        {
            Title = title,
            Body = body,
            Type = type,
            Data = new
            {
                Type = type,
                DeepLink = deepLink,
                WebUrl = webUrl,
                Data = data
            },
            DeepLink = deepLink,
            WebUrl = webUrl,
            TargetUserId = targetUserId?.ToString(),
            TargetAdmins = targetAdmins,
            AdminEmails = adminEmails,
            Priority = priority,
            Sound = "default",
            Badge = 1
        };
    }

    /// <summary>
    /// Yeni ilan admin bildirimi oluştur
    /// </summary>
    public object CreateNewAnimalAdminNotification(
        Guid animalId,
        string animalName,
        string animalType,
        decimal price,
        string city,
        string district,
        List<string> adminEmails)
    {
        var (deepLink, webUrl) = CreateAnimalLinks(animalId, isAdmin: true);

        return CreateNotificationPayload(
            title: "🆕 Yeni Hayvan İlanı",
            body: $"{animalName} - {price:N2} ₺ - {city}, {district}",
            type: "NewAnimalAdminNotification",
            deepLink: deepLink,
            webUrl: webUrl,
            data: new
            {
                AnimalId = animalId,
                AnimalName = animalName,
                AnimalType = animalType,
                Price = price,
                City = city,
                District = district
            },
            targetAdmins: true,
            adminEmails: adminEmails
        );
    }

    /// <summary>
    /// Yeni teklif bildirimi oluştur (Satıcıya)
    ///  Satıcı kendi ilanına gelen teklifi görür → /my-listings/{animalId}
    /// </summary>
    public object CreateNewBidNotification(
        Guid sellerId,
        Guid animalId,
        string animalName,
        decimal bidAmount,
        Guid bidderId)
    {
        // Satıcı kendi ilanının detayına gitsin
        var path = $"my-listings/{animalId}";
        var deepLink = $"{DeepLinkScheme}{path}";
        var webUrl = $"{_baseWebUrl}/{path}";

        return CreateNotificationPayload(
            title: "🎯 Yeni Teklif Aldınız!",
            body: $"{animalName} ilanınıza {bidAmount:N0} ₺ teklif geldi!",
            type: "NewBidReceived",
            deepLink: deepLink,
            webUrl: webUrl,
            data: new
            {
                AnimalId = animalId,
                AnimalName = animalName,
                BidAmount = bidAmount,
                BidderId = bidderId,
                SellerId = sellerId
            },
            targetUserId: sellerId
        );
    }

    /// <summary>
    /// Teklif geçildi bildirimi oluştur (Alıcıya)
    /// </summary>
    public object CreateBidOutbidNotification(
        Guid outbiddedUserId,
        Guid animalId,
        string animalName,
        decimal oldBidAmount,
        decimal newBidAmount)
    {
        var (deepLink, webUrl) = CreateAnimalLinks(animalId);

        return CreateNotificationPayload(
            title: "⚠️ Teklifiniz Geçildi!",
            body: $"{animalName} için verdiğiniz {oldBidAmount:N0} ₺ teklif, {newBidAmount:N0} ₺ ile geçildi.",
            type: "BidOutbid",
            deepLink: deepLink,
            webUrl: webUrl,
            data: new
            {
                AnimalId = animalId,
                AnimalName = animalName,
                OldBidAmount = oldBidAmount,
                NewBidAmount = newBidAmount
            },
            targetUserId: outbiddedUserId
        );
    }

    /// <summary>
    /// Teklif kabul edildi bildirimi oluştur (Alıcıya)
    ///  Alıcı direkt ödeme sayfasına yönlendirilir → /payment
    /// </summary>
    public object CreateBidAcceptedNotification(
        Guid buyerId,
        Guid animalId,
        string animalName,
        decimal bidAmount,
        Guid bidId,
        Guid sellerId)
    {
        // Ödeme sayfasına query parametreleri ile yönlendir
        var queryParams = $"?animalId={animalId}&bidId={bidId}&amount={bidAmount:F2}&animalName={Uri.EscapeDataString(animalName)}&sellerId={sellerId}";
        var path = $"payment{queryParams}";
        var deepLink = $"{DeepLinkScheme}{path}";
        var webUrl = $"{_baseWebUrl}/{path}";

        return CreateNotificationPayload(
            title: " Teklifiniz Kabul Edildi!",
            body: $"{animalName} için verdiğiniz {bidAmount:N0} ₺ teklif kabul edildi! Ödeme yapabilirsiniz.",
            type: "BidAccepted",
            deepLink: deepLink,
            webUrl: webUrl,
            data: new
            {
                AnimalId = animalId,
                AnimalName = animalName,
                BidAmount = bidAmount,
                BidId = bidId,
                SellerId = sellerId
            },
            targetUserId: buyerId
        );
    }

    /// <summary>
    /// İlan onaylandı bildirimi oluştur (Satıcıya)
    ///  Satıcı kendi ilanını görür → /my-listings/{animalId}
    /// </summary>
    public object CreateAnimalApprovedNotification(
        Guid sellerId,
        Guid animalId,
        string animalName)
    {
        // Satıcı kendi ilanının detayına gitsin
        var path = $"my-listings/{animalId}";
        var deepLink = $"{DeepLinkScheme}{path}";
        var webUrl = $"{_baseWebUrl}/{path}";

        return CreateNotificationPayload(
            title: "🎉 İlanınız Onaylandı!",
            body: $"{animalName} adlı ilanınız onaylandı ve yayında!",
            type: "AnimalListingApproved",
            deepLink: deepLink,
            webUrl: webUrl,
            data: new
            {
                AnimalId = animalId,
                AnimalName = animalName
            },
            targetUserId: sellerId
        );
    }

    /// <summary>
    /// İlan reddedildi bildirimi oluştur (Satıcıya)
    ///  Satıcı kendi ilanını görür → /my-listings/{animalId}
    /// </summary>
    public object CreateAnimalRejectedNotification(
        Guid sellerId,
        Guid animalId,
        string animalName,
        string rejectReason = "")
    {
        // Satıcı kendi ilanının detayına gitsin
        var path = $"my-listings/{animalId}";
        var deepLink = $"{DeepLinkScheme}{path}";
        var webUrl = $"{_baseWebUrl}/{path}";

        return CreateNotificationPayload(
            title: "İlanınız Reddedildi",
            body: string.IsNullOrEmpty(rejectReason)
                ? $"{animalName} adlı ilanınız reddedildi"
                : $"{animalName} adlı ilanınız reddedildi: {rejectReason}",
            type: "AnimalListingRejected",
            deepLink: deepLink,
            webUrl: webUrl,
            data: new
            {
                AnimalId = animalId,
                AnimalName = animalName,
                RejectReason = rejectReason
            },
            targetUserId: sellerId
        );
    }

    /// <summary>
    /// Veteriner onay talebi bildirimi oluştur (Veterinere)
    /// </summary>
    public object CreateVetApprovalRequestNotification(
        Guid vetId,
        Guid approvalId,
        Guid animalId,
        string animalName)
    {
        var (deepLink, webUrl) = CreateVetJobLinks(approvalId);

        return CreateNotificationPayload(
            title: "🩺 Yeni Onay Talebi",
            body: $"{animalName} için onay talebi aldınız",
            type: "VeterinaryApprovalCreated",
            deepLink: deepLink,
            webUrl: webUrl,
            data: new
            {
                ApprovalId = approvalId,
                AnimalId = animalId,
                AnimalName = animalName
            },
            targetUserId: vetId
        );
    }

    /// <summary>
    /// Veteriner onayı verildi bildirimi oluştur (İlan sahibine)
    ///  Satıcı kendi ilanını görür → /my-listings/{animalId}
    /// </summary>
    public object CreateVetApprovalGivenNotification(
        Guid sellerId,
        Guid animalId,
        string animalName,
        string vetName)
    {
        // Satıcı kendi ilanının detayına gitsin
        var path = $"my-listings/{animalId}";
        var deepLink = $"{DeepLinkScheme}{path}";
        var webUrl = $"{_baseWebUrl}/{path}";

        return CreateNotificationPayload(
            title: " Veteriner Onayı Verildi",
            body: $"{animalName} için {vetName} tarafından onay verildi",
            type: "VeterinaryApprovalApproved",
            deepLink: deepLink,
            webUrl: webUrl,
            data: new
            {
                AnimalId = animalId,
                AnimalName = animalName,
                VetName = vetName
            },
            targetUserId: sellerId
        );
    }

    /// <summary>
    /// Ödeme tamamlandı bildirimi oluştur
    /// </summary>
    public object CreatePaymentCompletedNotification(
        Guid userId,
        Guid animalId,
        string animalName,
        decimal amount,
        bool isSeller)
    {
        var (deepLink, webUrl) = isSeller
            ? CreateSalesLinks()
            : CreatePurchasesLinks();

        var title = isSeller ? "💰 Ödeme Alındı" : " Ödeme Tamamlandı";
        var body = isSeller
            ? $"{animalName} satışı için {amount:N2} ₺ ödeme alındı"
            : $"{animalName} için {amount:N2} ₺ ödeme tamamlandı";

        return CreateNotificationPayload(
            title: title,
            body: body,
            type: "PaymentCompleted",
            deepLink: deepLink,
            webUrl: webUrl,
            data: new
            {
                AnimalId = animalId,
                AnimalName = animalName,
                Amount = amount,
                IsSeller = isSeller
            },
            targetUserId: userId
        );
    }

    /// <summary>
    /// Teslimat başladı bildirimi oluştur
    /// </summary>
    public object CreateDeliveryStartedNotification(
        Guid userId,
        Guid deliveryId,
        Guid animalId,
        string animalName,
        bool isSeller)
    {
        var (deepLink, webUrl) = CreateDeliveryLinks(deliveryId);

        var body = isSeller
            ? $"{animalName} teslimatı başladı"
            : $"{animalName} teslimatı başladı, takip edebilirsiniz";

        return CreateNotificationPayload(
            title: "🚚 Teslimat Başladı",
            body: body,
            type: "DeliveryStarted",
            deepLink: deepLink,
            webUrl: webUrl,
            data: new
            {
                DeliveryId = deliveryId,
                AnimalId = animalId,
                AnimalName = animalName,
                IsSeller = isSeller
            },
            targetUserId: userId
        );
    }

    /// <summary>
    /// Teslimat tamamlandı bildirimi oluştur
    /// </summary>
    public object CreateDeliveryCompletedNotification(
        Guid userId,
        Guid deliveryId,
        Guid animalId,
        string animalName,
        bool isSeller)
    {
        var (deepLink, webUrl) = CreateDeliveryLinks(deliveryId);

        var body = isSeller
            ? $"{animalName} teslimatı tamamlandı"
            : $"{animalName} teslimatı tamamlandı, lütfen onaylayın";

        return CreateNotificationPayload(
            title: " Teslimat Tamamlandı",
            body: body,
            type: "DeliveryCompleted",
            deepLink: deepLink,
            webUrl: webUrl,
            data: new
            {
                DeliveryId = deliveryId,
                AnimalId = animalId,
                AnimalName = animalName,
                IsSeller = isSeller
            },
            targetUserId: userId
        );
    }
}