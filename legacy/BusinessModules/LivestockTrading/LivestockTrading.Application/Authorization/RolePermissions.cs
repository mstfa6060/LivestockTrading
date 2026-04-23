using LivestockTrading.Application.Constants;
using LivestockTrading.Domain.Enums;

namespace LivestockTrading.Application.Authorization;

/// <summary>
/// Roller için izin eşleştirmeleri
/// Her rol için hangi izinlerin geçerli olduğunu tanımlar
/// </summary>
public static class RolePermissions
{
    /// <summary>
    /// Rol bazlı izin eşleştirmeleri
    /// </summary>
    public static readonly Dictionary<string, Permission[]> Mappings = new()
    {
        // Admin - Tam yetki
        [LivestockTradingConstants.Roles.Admin] = Enum.GetValues<Permission>(),

        // Moderator - İçerik ve kullanıcı yönetimi
        [LivestockTradingConstants.Roles.Moderator] = new[]
        {
            // Kullanıcı görüntüleme ve ban
            Permission.UsersView,
            Permission.UsersBan,
            Permission.UsersUnban,

            // Ürün tam yönetim (onay dahil)
            Permission.ProductsView,
            Permission.ProductsUpdate,
            Permission.ProductsDelete,
            Permission.ProductsApprove,
            Permission.ProductsReject,
            Permission.ProductsUpdateStatus,

            // Kategori ve marka yönetimi
            Permission.CategoriesView,
            Permission.CategoriesCreate,
            Permission.CategoriesUpdate,
            Permission.CategoriesDelete,
            Permission.BrandsView,
            Permission.BrandsCreate,
            Permission.BrandsUpdate,
            Permission.BrandsDelete,

            // Satıcı/Nakliyeci doğrulama
            Permission.SellersView,
            Permission.SellersVerify,
            Permission.SellersSuspend,
            Permission.TransportersView,
            Permission.TransportersVerify,
            Permission.TransportersSuspend,

            // İçerik yönetimi
            Permission.BannersView,
            Permission.BannersCreate,
            Permission.BannersUpdate,
            Permission.BannersDelete,
            Permission.PagesView,
            Permission.PagesCreate,
            Permission.PagesUpdate,
            Permission.PagesDelete,

            // Değerlendirme onay
            Permission.ReviewsApprove,
            Permission.ReviewsDelete,

            // Raporlar
            Permission.ReportsPlatform,
            Permission.ReportsSales,
        },

        // Support - Müşteri destek
        [LivestockTradingConstants.Roles.Support] = new[]
        {
            // Kullanıcı görüntüleme
            Permission.UsersView,

            // Sipariş görüntüleme ve yönetim
            Permission.OrdersView,
            Permission.OrdersViewAll,
            Permission.OrdersUpdate,
            Permission.OrdersCancel,

            // Ürün görüntüleme
            Permission.ProductsView,

            // Nakliye görüntüleme
            Permission.TransportRequestsView,
            Permission.TransportOffersView,
            Permission.TransportUpdateStatus,

            // Ticket yönetimi
            Permission.TicketsView,
            Permission.TicketsRespond,
            Permission.TicketsClose,
        },

        // Seller - Satıcı
        [LivestockTradingConstants.Roles.Seller] = new[]
        {
            // Kendi ürün yönetimi
            Permission.ProductsView,
            Permission.ProductsCreate,
            Permission.ProductsUpdate,
            Permission.ProductsDelete,
            Permission.ProductsManageOwn,

            // Kendi sipariş yönetimi (satılan)
            Permission.OrdersView,
            Permission.OrdersUpdate,
            Permission.OrdersManageOwn,

            // Kendi çiftlik yönetimi
            Permission.FarmsView,
            Permission.FarmsCreate,
            Permission.FarmsUpdate,
            Permission.FarmsDelete,
            Permission.FarmsManageOwn,

            // Kendi satıcı profili
            Permission.SellersView,
            Permission.SellersUpdate,
            Permission.SellersManageOwn,

            // Nakliye talebi oluşturma
            Permission.TransportRequestsView,
            Permission.TransportRequestsCreate,
            Permission.TransportOffersView,
            Permission.TransportOffersAccept,
            Permission.TransportOffersReject,

            // Raporlar (kendi)
            Permission.ReportsSales,
            Permission.ReportsFinancial,

            // Ticket
            Permission.TicketsCreate,
            Permission.TicketsManageOwn,
        },

        // Transporter - Nakliyeci
        [LivestockTradingConstants.Roles.Transporter] = new[]
        {
            // Kendi nakliyeci profili
            Permission.TransportersView,
            Permission.TransportersUpdate,
            Permission.TransportersManageOwn,

            // Nakliye işlemleri
            Permission.TransportRequestsView,
            Permission.TransportOffersView,
            Permission.TransportOffersCreate,
            Permission.TransportUpdateStatus,
            Permission.TransportManageOwn,

            // Raporlar (kendi)
            Permission.ReportsTransport,
            Permission.ReportsFinancial,

            // Ticket
            Permission.TicketsCreate,
            Permission.TicketsManageOwn,
        },

        // Buyer - Alıcı (varsayılan rol)
        [LivestockTradingConstants.Roles.Buyer] = new[]
        {
            // Ürün ve kategori görüntüleme
            Permission.ProductsView,
            Permission.CategoriesView,
            Permission.BrandsView,

            // Sipariş oluşturma ve yönetme
            Permission.OrdersCreate,
            Permission.OrdersView,
            Permission.OrdersCancel,
            Permission.OrdersManageOwn,

            // Nakliye talebi
            Permission.TransportRequestsCreate,
            Permission.TransportRequestsView,
            Permission.TransportOffersView,
            Permission.TransportOffersAccept,
            Permission.TransportOffersReject,

            // Favoriler ve değerlendirmeler
            Permission.FavoritesManage,
            Permission.ReviewsCreate,
            Permission.ReviewsUpdate,

            // Satıcı/Nakliyeci profil görüntüleme
            Permission.SellersView,
            Permission.TransportersView,
            Permission.FarmsView,

            // Ticket
            Permission.TicketsCreate,
            Permission.TicketsManageOwn,
        },

        // Veterinarian - Veteriner
        [LivestockTradingConstants.Roles.Veterinarian] = new[]
        {
            // Sağlık sertifikası ve rapor
            Permission.HealthCertificatesView,
            Permission.HealthCertificatesCreate,
            Permission.HealthCertificatesApprove,
            Permission.HealthReportsCreate,
            Permission.VeterinaryDocumentsManage,

            // Ürün ve çiftlik görüntüleme (muayene için)
            Permission.ProductsView,
            Permission.FarmsView,
            Permission.SellersView,

            // Ticket
            Permission.TicketsCreate,
            Permission.TicketsManageOwn,
        },
    };

    /// <summary>
    /// Belirtilen rolün belirtilen izne sahip olup olmadığını kontrol eder
    /// </summary>
    public static bool HasPermission(string role, Permission permission)
    {
        if (string.IsNullOrWhiteSpace(role))
            return false;

        if (!Mappings.TryGetValue(role, out var permissions))
            return false;

        // Admin her zaman tam yetkili
        if (role == LivestockTradingConstants.Roles.Admin)
            return true;

        return permissions.Contains(permission);
    }

    /// <summary>
    /// Belirtilen rollerin herhangi birinin belirtilen izne sahip olup olmadığını kontrol eder
    /// </summary>
    public static bool HasAnyPermission(IEnumerable<string> roles, Permission permission)
    {
        return roles.Any(role => HasPermission(role, permission));
    }

    /// <summary>
    /// Belirtilen rollerin tüm belirtilen izinlere sahip olup olmadığını kontrol eder
    /// </summary>
    public static bool HasAllPermissions(IEnumerable<string> roles, params Permission[] permissions)
    {
        return permissions.All(p => HasAnyPermission(roles, p));
    }

    /// <summary>
    /// Belirtilen rol için tüm izinleri getirir
    /// </summary>
    public static Permission[] GetPermissions(string role)
    {
        if (string.IsNullOrWhiteSpace(role))
            return Array.Empty<Permission>();

        return Mappings.TryGetValue(role, out var permissions)
            ? permissions
            : Array.Empty<Permission>();
    }

    /// <summary>
    /// Belirtilen roller için birleştirilmiş izinleri getirir
    /// </summary>
    public static Permission[] GetCombinedPermissions(IEnumerable<string> roles)
    {
        return roles
            .SelectMany(GetPermissions)
            .Distinct()
            .ToArray();
    }
}
