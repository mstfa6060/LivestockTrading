# Migration Gaps — ArfBlocks (legacy) vs FastEndpoints (src/) Karşılaştırması

**Tarih:** 2026-04-22
**Durum:** PR#1–#7 tamamlandı, src/ altında 183 endpoint çalışıyor.
**Bu rapor:** Eski (legacy/) yapı ile yeni (src/) yapı arasındaki **kapsam ve davranış farklarını** belgeler.

---

## Özet

| Metrik | Değer |
|---|---|
| Eski toplam endpoint | 217 |
| Yeni toplam endpoint | 183 |
| Net fark | -34 |
| Kritik P0 düzeltme | 6 |
| P1 önemli düzeltme | 5 alan |
| P2 içerik yönetimi | 7 alan |

Net fark yanıltıcı: pek çok endpoint rename + merge oldu, gerçek eksik daha derin.

---

## Bölüm A — Endpoint Kapsam Delta'sı

### A.1 IAM Modülü

**Eski:** 30 endpoint, **Yeni:** 15 endpoint.

#### Eski'de var, yeni'de YOK (15 endpoint)

- `Auth/RevokeRefreshToken` — Refresh token iptal etme; güvenlik için kritik
- `Push/RegisterToken` — Push bildirim token kaydı; mobil bildirimler için kritik
- `Push/RevokeToken` — Push token silme
- `Countries/All` — Ülke listesi; kayıt/adres formları için
- `Provinces/All` — İl listesi
- `Districts/ByProvince` — İlçe listesi
- `Neighborhoods/ByDistrict` — Mahalle listesi
- `GeoIp/DetectCountry` — IP'den ülke tespiti; onboarding
- `MobilApplicationVersiyon/GetVersion` — Mobil sürüm kontrolü
- `Role/Create, Update, Delete, All` — Rol yönetimi (4 endpoint)
- `User/Create, Delete, Update, UpdatePassword, Detail, ExportData` — Admin user CRUD
- `UserPermissions/All` — İzin sorgulama

#### Yeni'de var, eski'de yok (3 endpoint)

- `/iam/Auth/Register`
- `/iam/Admin/Users/Ban`
- `/iam/Admin/Users/Unban`

### A.2 Files / FileProvider Modülü

**Eski:** 7 endpoint, **Yeni:** 7 endpoint (ama farklılar).

#### Eski'de var, yeni'de YOK (5 endpoint)

- `Bucket/Copy`, `Bucket/Duplicate`, `Bucket/Detail`
- `File/UploadEyp` — EYP (Türkiye'ye özgü elektronik belge formatı)
- `File/UploadToApprovedBucket` — Onaylı bucket'a yükleme

#### Yeni'de var, eski'de yok (5 endpoint)

- `/fileprovider/Buckets/Create`, `/fileprovider/Buckets/Delete`
- `/fileprovider/Files/PresignedUrl` — S3 presigned URL üretme
- `/fileprovider/Files/Reorder`, `/fileprovider/Files/SetCover`

### A.3 LivestockTrading Modülü

**Eski:** 180 endpoint, **Yeni:** 161 endpoint.

#### Tamamen taşınmamış domain'ler

| Alan | Eksik endpoint | Etki |
|---|---|---|
| **Shipping** (Carriers + Rates + Zones) | 18 | Kargo akışı implement edilemez |
| **Banners** | 6 | İçerik yönetimi yok |
| **FAQs** | 6 | SSS yok |
| **Languages** | 6 | Dil yönetimi yok |
| **PaymentMethods** | 6 | Ödeme yöntemi yönetimi yok |
| **TaxRates** | 6 | Vergi hesaplaması eksik |
| **ProductPrices** | 6 | (ProductVariants ile çakışır mı doğrulanmalı) |
| **ProductImages** | 3 | Görsel yönetimi endpoint'i yok |
| **ContactForms** | 1 | İletişim kanalı yok |

#### Kısmen eksik

- **Sellers**: `Delete`, `Nearby` (geo-search), `DetailByUserId`, `GetByUserId`, `Pick` eksik
- **TransportOffers + TransportTrackings**: `Update`, `Delete`, `All`, `Detail` eksik
- **Reviews**: `Update`, `Delete` eksik (kullanıcı yorumu düzenleyemez/silemez)
- **Conversations / Messages**: `UnreadCount`, `Update`, `Delete`, `Pick`, `SendTypingIndicator` eksik
- **ProductBoosts**: `Expire`, `All` eksik (boost süresi dolanları işleyen endpoint yok)
- **Currencies**: `Convert`, `UpdateRates` eksik
- **Favorites**: `Update`, `Detail` eksik (FavoriteProducts → Favorites rename)
- **SearchHistories**: `Detail`, `Update` eksik
- **UserPreferences**: `GetTimeZone` eksik

#### Yeni'de eklenen iyileştirmeler

- `Dashboard/SellerStats` — satıcıya özel dashboard
- `Sellers/Me`, `Transporters/Me` — kendi profil
- `Admin/Sellers/Pending`, `Admin/Transporters/Pending` — onay kuyruğu
- `Admin/Reports/Resolve` — şikayet çözümleme
- `Boosts/Packages, Purchase` — boost satın alma akışı
- `Subscriptions/Plans, My, Subscribe` — abonelik akışı
- `Offers/Accept, Reject` — teklif kabul/red (legacy'de sadece Update)
- `Deals/UpdateStatus` — işlem durumu güncelleme
- `Conversations/MarkRead`, `Notifications/MarkRead, MarkAllRead`
- `Reviews/ByTransporter` — taşıyıcı yorumları
- `TransportRequests/Open` — açık taşıma talepleri

#### Rename tespitleri

- `FavoriteProducts` → `Favorites`
- `ProductReviews + SellerReviews` → `Reviews` (merge)
- `SellerSubscriptions` → `Subscriptions` (yeniden tasarlandı)
- `ProductBoosts` → `Boosts` (merge + kısmi yeniden yapılandırma)
- `SellerSubscriptions/Create` → `Subscriptions/Subscribe`
- `Sellers/Create` → `Sellers/Register`

---

## Bölüm B — Davranış (Logic) Delta'sı

### B.1 Auth akışı (mobil/web giriş bozulur)

**1. Login: Google/Apple OAuth tamamen yok**
- Eski: native + Google + Apple destekli; ilk girişte otomatik kullanıcı oluşturma, native hesabı OAuth'a bağlama. ([legacy/BaseModules/IAM/.../Login/Handler.cs:27–190])
- Yeni: yalnızca native (email+şifre). [Endpoint.cs:28–83](src/Modules/Iam/Iam.Features/Auth/Login/Endpoint.cs#L28)
- **Risk:** OAuth kullanıcıları sisteme giremez.

**2. Login: UserName desteği kaybedildi**
- Eski: `GetUser(request.UserName)` — username veya email
- Yeni: `u.Email == req.Email` — sadece email
- **Risk:** UserName ile kayıtlı kullanıcılar giremez.

**3. Register: Admin email whitelist eksik**
- Eski: `nagehanyazici13@gmail.com`, `m.mustafaocak@gmail.com` → otomatik Admin
- Yeni: tüm yeni kayıtlar hardcoded `BuyerRoleId`
- **Risk:** Admin paneli erişilemez.

**4. ResetPassword: token + email çifti zorunlu**
- Eski: sadece token ile sorgu
- Yeni: `u.Email == req.Email && u.PasswordResetToken == req.Token`
- **Risk:** Eski sıfırlama linkleri sadece token içerdiği için yeni endpoint reddeder.

**5. SendOtp: request modeli `UserId` → `PhoneNumber`**
- **Risk:** Eski mobil client uyumsuz.

**6. SendEmailOtp: `Language` alanı kaldırıldı**
- **Risk:** TR/EN/AR/DE/FR lokalize email kaybı.

**7. ForgotPassword: token süresi 15 dk → 1 saat**
- **Risk:** Süre beklentisi farklı; eski email içeriği farklı olabilir.

**8. SendOtp (eski): OTP kodu response'ta dönüyordu** — yeni'de güvenlik açığı kapatılmış (iyileştirme).

### B.2 Para/abonelik (gelir akışı bozulur)

**9. Subscriptions/Subscribe: IAP receipt doğrulaması yok**
- Eski: `Receipt` zorunlu, `RequireSeller()` rol kontrolü
- Yeni: Buyer da subscribe edebilir, ödeme doğrulaması yok
- **Risk:** IAP fraud riski, abuse açık.

**10. Subscription limit varsayılanı: 3 → 5**
- Eski: Free plan 3 ilan
- Yeni: Free plan 5 ilan
- **Risk:** İş kuralı farkı.

**11. ProductBoosts/Expire eksik**
- Boost süresi dolanları işleyen endpoint yok.

### B.3 Ürün akışı

**12. Products/Create: LocationId zorunluluğu kaldırıldı**
- **Risk:** Coğrafi arama bozulur.

**13. Products/Create: Admin in-app bildirimi gitmiyor**
- Eski: admin/moderatör'e push event'i
- Yeni: sadece NATS event publish
- **Risk:** Moderasyon kuyruğuna haber gelmez.

**14. Products/Approve: sosyal medya otopaylaşım event'i kaldırıldı**

**15. Products/Approve: timezone hesabı farklı**
- Eski: satıcının kişisel timezone'u DB'den çekilir
- Yeni: satıcının lokasyon `CountryCode`'una göre

### B.4 Satıcı/teklif

**16. Sellers/Verify ve Suspend: `IsVerified` ve `IsActive` flag'leri güncellenmiyor**
- **Risk:** Bu alanlara bağlı tüm filtreleme sorguları hatalı çalışır.

**17. Sellers/Register: rol ataması NATS consumer'a taşındı (atomic değil)**
- **Risk:** Consumer geç/başarısız olursa kullanıcı seller ama Seller rolü yok (race condition).

**18. Offers/Create: rol kontrolü kaldırıldı**
- Eski: Buyer/Seller/Admin/Moderator zorunlu
- Yeni: herhangi bir authenticated kullanıcı offer yapabilir.

**19. Offers/Create: ExpiresAt yeni'de hardcoded 3 gün**

**20. Offers/Accept: Deal oluşturma + stok azaltma (yeni iyileştirme)**

### B.5 Mesajlaşma

**21. Conversations/Create: duplicate önleme (yeni iyileştirme)**

**22. Conversations: SignalR/ChatHub yok**
- Eski: `ChatHub` ile join, typing indicator, presence (Redis)
- Yeni: yok (CLAUDE.md `/hubs/chat` diyor ama kodda yok)
- **Risk:** Real-time mesajlaşma, typing indicator, presence çalışmaz.

### B.6 Files

**23. Upload: image processing pipeline kaldırıldı**
- Eski: WebP, thumbnail, metadata stripping, %85 kalite, varyantlar
- Yeni: ham binary upload
- **Risk:** Storage maliyeti artar, thumbnail'a ihtiyacı olan UI'lar boş kalır.

---

## Bölüm C — Genel Desen Farkları

1. **Event publish:** RabbitMQ Fanout → NATS Point-to-Point. Subject ismi: eski `livestocktrading.*`, yeni `livestock.*`. Eski worker'lar yeni event'leri dinleyemez.

2. **Validation:** Verificator (DB-bound check) → Handler içi inline check'e taşındı. Bazı önemli kontroller atlandı (özellikle rol kontrolleri Offers/Create'de).

3. **Cache:** Eski'de `ICacheService` (Redis) ile `Provinces/All`, `Districts/ByProvince` cacheliyordu. Yeni endpoint'lerde FusionCache hiç kullanılmıyor — referans veriler her istekte DB'ye gidiyor.

4. **Auth:** Eski multi-provider (native/Google/Apple), yeni sadece native.

5. **Audit log:** Eski'de `AuditLogService` + `AppAuditLogs` tablosu vardı. Yeni mimaride yok. Compliance riski.

6. **SignalR:** Eski `ChatHub` (join, typing, presence + Redis). Yeni'de hiç yok.

7. **File upload:** Image processing pipeline kaldırıldı.

8. **Subscriptions:** `RequireSeller()` ve `Receipt` zorunluluğu eski'de vardı, yeni'de hiç biri yok.

9. **Seller status:** `IsActive` ve `IsVerified` flag'leri yeni'de güncellenmediği için bu alanlara bağlı sorgular hatalı.

10. **OTP generation:** Eski `new Random()` (tahmin edilebilir), yeni `RandomNumberGenerator` (kriptografik) — iyileştirme.

---

## Öncelikli Aksiyon Listesi

### P0 — Şimdi düzeltilmeli (canlıya çıkmadan)

1. **IAP receipt validation** — `Subscribe` endpoint'i (Receipt zorunlu, RequireSeller())
2. **Sellers Verify/Suspend flag düzeltmesi** — `IsActive`, `IsVerified` alanlarını güncelle
3. **Push token Register/Revoke** — Mobil push tamamen kapalı
4. **Login UserName + OAuth (Google/Apple) desteği**
5. **Admin email whitelist** — Register endpoint'i
6. **SignalR ChatHub** — Real-time mesajlaşma

### P1 — Önemli, sprint içinde

- Shipping domain (Carriers + Rates + Zones) — 18 endpoint
- Sellers/Nearby — geo-search
- Reviews/Update + Delete
- Image processing pipeline (WebP, thumbnail)
- Conversations/UnreadCount

### P2 — İçerik yönetimi (admin paneli için)

- Banners, FAQs, Languages, PaymentMethods, TaxRates
- ContactForms
- GeoIp + Geolocation referans verileri (Countries/Provinces/Districts/Neighborhoods)
- Audit log altyapısı
- FusionCache entegrasyonu (referans veriler için)
