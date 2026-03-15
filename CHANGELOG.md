# CHANGELOG

Bu dosya, projeye eklenen tüm özellik, düzeltme ve iyileştirmeleri özetler.

---

## Global Revenue Strategy (claude/global-revenue-strategy-LU8kH)

**Toplam:** 58 commit | **Tarih:** Mart 2026

---

### 1. Monetizasyon Sistemi (IAP & Boost)

| Özellik | Açıklama |
|---------|----------|
| **IAP Abonelik Sistemi** | In-App Purchase abonelik yönetimi (SubscriptionPlan, UserSubscription entity'leri) |
| **Boost Paketleri** | Ürün öne çıkarma paketleri (BoostPackage, ProductBoost entity'leri) |
| **Listeleme Limitleri** | Ücretsiz/premium kullanıcılar için ilan limiti kontrolü (ListingLimit entity) |
| **BoostPackages/All** | Aktif boost paketlerini listeleme (public, çok dilli) |
| **ProductBoosts/Create** | Satıcının ürününe boost uygulaması |
| **ProductBoosts/All** | Satıcının boost geçmişini görüntülemesi |

### 2. Döviz Kuru & Para Birimi Sistemi

| Özellik | Açıklama |
|---------|----------|
| **Currency Seed Data** | 48 para birimi ve başlangıç kur verileri |
| **Exchange Rate Auto-Update** | Hangfire ile otomatik döviz kuru güncelleme job'ı |
| **Currencies/Convert** | Para birimi dönüştürme endpoint'i |
| **Fiyat Dönüştürme** | Ürün fiyatlarını kullanıcı para birimine çevirme |

### 3. Yetkilendirme & Güvenlik (RBAC)

| Özellik | Açıklama |
|---------|----------|
| **Rol Tabanlı Yetkilendirme** | Admin, Moderator, Seller, Transporter, Buyer rolleri |
| **PermissionService** | `RequireAdmin()`, `RequireModerator()`, `RequireSeller()` vb. |
| **Auto Admin Role** | Belirli e-posta adresleri için otomatik Admin rolü |
| **Auto Seller Role** | Satıcı profili oluşturulduğunda otomatik Seller rolü |
| **Public Endpoint'ler** | Ocelot gateway'de auth gerektirmeyen route'lar |

### 4. Gerçek Zamanlı Mesajlaşma (SignalR)

| Özellik | Açıklama |
|---------|----------|
| **ChatHub** | SignalR hub (`/hubs/chat`) - JoinConversation, SendTypingIndicator, MarkMessageAsRead |
| **Conversations CRUD** | Konuşma başlatma, listeleme, detay |
| **Messages CRUD** | Mesaj gönderme, listeleme, güncelleme |
| **PresenceService** | Redis ile online/offline durum takibi |
| **Messages/UnreadCount** | Okunmamış mesaj sayısı endpoint'i |
| **Typing Indicator** | Yazıyor göstergesi |

### 5. Bildirim Sistemi

| Özellik | Açıklama |
|---------|----------|
| **Firebase Push Notifications** | FCM ile mobil push bildirim |
| **RevokeToken Endpoint** | Logout'ta FCM token iptal etme |
| **Admin Bildirim** | Yeni ürün oluşturulduğunda admin'e bildirim |
| **NotificationSender Worker** | Push notification ve real-time event worker |
| **Notifications/All** | `actionUrl` ve `actionData` response alanları |

### 6. Sosyal Medya Entegrasyonu

| Özellik | Açıklama |
|---------|----------|
| **SocialMediaWorker** | Ürün onaylandığında Instagram'a otomatik paylaşım |
| **Instagram Token Refresh** | Otomatik Instagram token yenileme |
| **Cover Image Resolve** | FileProvider API ile kapak görseli çözümleme |
| **Web/API URL Ayrımı** | Instagram caption'larında ayrı web ve API URL'leri |

### 7. Ürün Yönetimi İyileştirmeleri

| Özellik | Açıklama |
|---------|----------|
| **Products/Search** | Ürün arama endpoint'i |
| **Products/DetailBySlug** | Slug ile ürün detayı |
| **Products/Approve & Reject** | Moderasyon endpoint'leri |
| **CategoryId Filtreleme** | Alt kategorileri otomatik dahil etme (Products/All & Search) |
| **CountryCode Filtreleme** | Ülke bazlı ürün listeleme |
| **Otomatik Onaya Gönderme** | Kullanıcı ilan girdiğinde otomatik onay kuyruğu |
| **Silinmiş Ürün Gizleme** | Sadece Admin/Moderator silinmiş ürünleri görebilir |
| **Cover Image URL** | Products/All ve Search response'larında kapak görseli |
| **mediaBucketId** | Products Detail ve All response'larında medya bucket bilgisi |

### 8. Kategori & Çeviri Sistemi

| Özellik | Açıklama |
|---------|----------|
| **Category Seed Data** | 40+ dilde kategori çeviri verileri |
| **ProductCount** | Categories/All'da her kategorinin aktif ürün sayısı |
| **Alfabetik Sıralama** | Kategorileri seçili dilde alfabetik sıralama |
| **Çoklu Dil Desteği** | TranslationHelper ile dinamik çeviri çözümleme |
| **8 Yeni Dil** | pt-BR, zh-CN, zh-TW ve 5 ek dil çevirisi |

### 9. Kullanıcı & Satıcı Yönetimi

| Özellik | Açıklama |
|---------|----------|
| **Users/Update** | Kullanıcı profil güncelleme endpoint'i |
| **Sellers/GetByUserId** | UserId ile satıcı bilgisi sorgulama |
| **Sellers/Verify & Suspend** | Satıcı doğrulama ve askıya alma |
| **Duplicate Seller Fix** | Aynı kullanıcı için çift satıcı oluşturma engeli |
| **Social Login Fix** | Mevcut e-posta hesaplarını bağlama (duplicate engeli) |
| **Social Login Fix** | CountryId ve CreatedAt eksikliği düzeltmesi |
| **Dashboard/Stats** | Admin dashboard istatistik endpoint'i |

### 10. Altyapı & DevOps

| Özellik | Açıklama |
|---------|----------|
| **Hangfire Scheduler** | Docker setup ve Jenkins pipeline entegrasyonu |
| **Dockerfile.migration-job** | Migration'ları Docker'da çalıştırma |
| **MinIO File Storage** | FileProvider için MinIO/S3 desteği |
| **MongoDB Service** | FileProvider doküman saklama |
| **Zero-Downtime Deploy** | Prod deploy'da `docker compose down` kaldırıldı |
| **Jenkins Pipeline Optimizasyonu** | Paralel build ile pipeline hızlandırma |
| **HANGFIRE_PORT** | `.env.example`'a Hangfire port tanımı |

### 11. Bug Fix'ler

| Fix | Açıklama |
|-----|----------|
| **Products/Create 500** | Handler'daki gereksiz ILogger dependency kaldırıldı |
| **Products/All 500** | FileEntries SQL sorgusu kaynaklı hata düzeltildi |
| **IAM Workers Crash** | Eksik config fallback'leri eklendi |
| **IAM Workers RabbitMQ** | Yanlış env variable isimleri düzeltildi |
| **SocialMediaWorker RabbitMQ** | Config key uyumsuzluğu düzeltildi |
| **Approve Handler Crash** | FileEntries tablosu eksikken oluşan hata düzeltildi |
| **Sellers/Create** | Rol atamada try-catch eklendi |
| **Products/All** | Aktif olmayan ürünlerin listelenmesi engellendi |

### 12. Diğer İyileştirmeler

| İyileştirme | Açıklama |
|-------------|----------|
| **ErrorCodeExporter** | Sadece `tr.ts` dosyası üretecek şekilde sadeleştirildi |
| **Worker Projeleri** | LivestockTrading worker projeleri solution'a eklendi |
| **Request Property Standardizasyonu** | Request model'lerde tutarlı property isimlendirmesi |

---

### Teknik Özet

| Metrik | Değer |
|--------|-------|
| Toplam Commit | 58 |
| Yeni Entity'ler | SubscriptionPlan, UserSubscription, BoostPackage, ProductBoost, ListingLimit, Currency, ExchangeRate |
| Yeni Worker'lar | SocialMediaWorker, NotificationSender, HangfireScheduler |
| Yeni Endpoint'ler | ~25+ |
| Desteklenen Diller | 40+ |
| Desteklenen Para Birimleri | 48 |
| SignalR Hub | ChatHub (/hubs/chat) |
| Altyapı Servisleri | MinIO, MongoDB, Hangfire, Firebase |
