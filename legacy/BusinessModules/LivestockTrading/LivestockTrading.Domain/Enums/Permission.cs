namespace LivestockTrading.Domain.Enums;

/// <summary>
/// Platform genelinde kullanılan izin tanımları
/// Her kategori 100'er aralıklarla numaralandırılmıştır
/// </summary>
public enum Permission
{
    // ============================================
    // Kullanıcı Yönetimi (100-199)
    // ============================================
    UsersView = 100,
    UsersCreate = 101,
    UsersUpdate = 102,
    UsersDelete = 103,
    UsersBan = 104,
    UsersUnban = 105,
    UsersAssignRole = 106,
    UsersRemoveRole = 107,

    // ============================================
    // Ürün Yönetimi (200-299)
    // ============================================
    ProductsView = 200,
    ProductsCreate = 201,
    ProductsUpdate = 202,
    ProductsDelete = 203,
    ProductsApprove = 204,
    ProductsReject = 205,
    ProductsUpdateStatus = 206,
    ProductsManageOwn = 207,  // Kendi ürünlerini yönetme

    // ============================================
    // Sipariş Yönetimi (300-399)
    // ============================================
    OrdersView = 300,
    OrdersCreate = 301,
    OrdersUpdate = 302,
    OrdersCancel = 303,
    OrdersViewAll = 304,     // Tüm siparişleri görme
    OrdersManageOwn = 305,   // Kendi siparişlerini yönetme

    // ============================================
    // Kategori & Marka Yönetimi (400-499)
    // ============================================
    CategoriesView = 400,
    CategoriesCreate = 401,
    CategoriesUpdate = 402,
    CategoriesDelete = 403,
    BrandsView = 410,
    BrandsCreate = 411,
    BrandsUpdate = 412,
    BrandsDelete = 413,

    // ============================================
    // Nakliye Yönetimi (500-599)
    // ============================================
    TransportRequestsView = 500,
    TransportRequestsCreate = 501,
    TransportRequestsUpdate = 502,
    TransportRequestsCancel = 503,
    TransportOffersView = 510,
    TransportOffersCreate = 511,
    TransportOffersAccept = 512,
    TransportOffersReject = 513,
    TransportUpdateStatus = 520,
    TransportManageOwn = 521,  // Kendi nakliyelerini yönetme

    // ============================================
    // Satıcı Yönetimi (600-699)
    // ============================================
    SellersView = 600,
    SellersCreate = 601,
    SellersUpdate = 602,
    SellersDelete = 603,
    SellersVerify = 604,
    SellersSuspend = 605,
    SellersManageOwn = 606,   // Kendi satıcı profilini yönetme

    // ============================================
    // Nakliyeci Yönetimi (700-799)
    // ============================================
    TransportersView = 700,
    TransportersCreate = 701,
    TransportersUpdate = 702,
    TransportersDelete = 703,
    TransportersVerify = 704,
    TransportersSuspend = 705,
    TransportersManageOwn = 706,  // Kendi nakliyeci profilini yönetme

    // ============================================
    // Çiftlik Yönetimi (800-899)
    // ============================================
    FarmsView = 800,
    FarmsCreate = 801,
    FarmsUpdate = 802,
    FarmsDelete = 803,
    FarmsManageOwn = 804,

    // ============================================
    // Veteriner/Sağlık Yönetimi (900-999)
    // ============================================
    HealthCertificatesView = 900,
    HealthCertificatesCreate = 901,
    HealthCertificatesApprove = 902,
    HealthReportsCreate = 903,
    VeterinaryDocumentsManage = 904,

    // ============================================
    // İçerik Yönetimi (1000-1099)
    // ============================================
    BannersView = 1000,
    BannersCreate = 1001,
    BannersUpdate = 1002,
    BannersDelete = 1003,
    PagesView = 1010,
    PagesCreate = 1011,
    PagesUpdate = 1012,
    PagesDelete = 1013,

    // ============================================
    // Favoriler & Değerlendirmeler (1100-1199)
    // ============================================
    FavoritesManage = 1100,
    ReviewsCreate = 1101,
    ReviewsUpdate = 1102,
    ReviewsDelete = 1103,
    ReviewsApprove = 1104,

    // ============================================
    // Raporlar & İstatistikler (1200-1299)
    // ============================================
    ReportsPlatform = 1200,
    ReportsSales = 1201,
    ReportsTransport = 1202,
    ReportsFinancial = 1203,

    // ============================================
    // Destek & Ticket (1300-1399)
    // ============================================
    TicketsView = 1300,
    TicketsCreate = 1301,
    TicketsRespond = 1302,
    TicketsClose = 1303,
    TicketsManageOwn = 1304,

    // ============================================
    // Sistem Yönetimi (9000-9999)
    // ============================================
    SystemSettings = 9000,
    SystemLogs = 9001,
    SystemBackup = 9002,
    FullAccess = 9999  // Admin tam yetki
}
