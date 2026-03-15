# Gelir Modeli - Frontend & Mobil Entegrasyon Talimatlari

**Tarih:** 15 Mart 2026
**Branch:** `claude/global-revenue-strategy-LU8kH`
**Hazirlayan:** Backend Ekibi
**Hedef:** Web Frontend & Mobil Ekipleri

---

## Genel Bakis

Backend tarafinda platformun gelir modelini olusturan 3 ana sistem gelistirildi:

| # | Sistem | Aciklama | Backend Durumu |
|---|--------|----------|----------------|
| 1 | **Abonelik Planlari** | Saticilar icin Free/Basic/Pro/Business planlari, ilan limiti | Hazir |
| 2 | **Boost Paketleri** | Urun one cikarma (Daily/Weekly/Mega) | Hazir |
| 3 | **IAP Islem Kaydi** | App Store / Google Play satin alma islem takibi | Hazir |

**Kisaca akis:**
- Satici ucretsiz plan ile baslar (3 ilan limiti)
- Daha fazla ilan icin plan yukseltir (IAP ile satin alir)
- Urunlerini one cikarmak icin boost paketi satin alir (IAP ile)
- Tum odemeler App Store / Google Play / Web uzerinden yapilir

---

## 1. ABONELIK PLANLARI

### 1.1 Planlari Listeleme

**Endpoint:** `POST /livestocktrading/SubscriptionPlans/All`
**Auth:** Gerekmez (Public)

```json
// Request
{
  "languageCode": "tr",
  "targetType": 0       // opsiyonel - 0=Seller, 1=Transporter
}

// Response
{
  "data": [
    {
      "id": "plan-guid",
      "name": "Pro Plan",                         // languageCode'a gore cevrilmis
      "description": "Profesyonel saticilar icin",
      "targetType": 0,
      "tier": 2,                    // 0=Free, 1=Basic, 2=Pro, 3=Business
      "priceMonthly": 9.99,
      "priceYearly": 99.99,
      "currency": "USD",
      "appleProductIdMonthly": "com.livestock.pro.monthly",
      "appleProductIdYearly": "com.livestock.pro.yearly",
      "googleProductIdMonthly": "pro_monthly",
      "googleProductIdYearly": "pro_yearly",
      "maxActiveListings": 50,      // 0 = sinirsiz
      "maxPhotosPerListing": 20,
      "monthlyBoostCredits": 5,
      "hasDetailedAnalytics": true,
      "hasPrioritySupport": true,
      "hasFeaturedBadge": true,
      "sortOrder": 2,
      "createdAt": "2026-03-15T..."
    }
  ]
}
```

**Plan Detayi:** `POST /livestocktrading/SubscriptionPlans/Detail` (Public)
```json
// Request
{ "id": "plan-guid", "languageCode": "tr" }
```

**UI Gereksinimleri:**
- Karsilastirmali plan tablosu (4 kolon: Free / Basic / Pro / Business)
- Aylik / Yillik toggle (yillik secenekte indirim vurgulama)
- Her planin ozelliklerini tik/carpı ile goster:
  - Ilan limiti (`maxActiveListings`, 0 = "Sinirsiz")
  - Fotograf limiti (`maxPhotosPerListing`)
  - Aylik boost kredisi (`monthlyBoostCredits`)
  - Detayli analitik (`hasDetailedAnalytics`)
  - Oncelikli destek (`hasPrioritySupport`)
  - One Cikan Satici rozeti (`hasFeaturedBadge`)
- Kullanicinin mevcut plani vurgulama
- "Satin Al" / "Yukselt" butonu

---

### 1.2 Abonelik Satin Alma

**Endpoint:** `POST /livestocktrading/SellerSubscriptions/Create`
**Auth:** Gerekli (Seller rolu)

**Akis:**
1. Kullanici plan secer, Aylik/Yillik secer
2. Platform'a gore IAP akisi baslar
3. Store'dan receipt/token alinir
4. Backend'e gonderilir:

```json
// Request
{
  "sellerId": "satici-guid",
  "subscriptionPlanId": "plan-guid",
  "period": 0,                      // 0=Monthly, 1=Yearly
  "platform": 0,                    // 0=Apple, 1=Google, 2=Web
  "receipt": "base64-encoded-receipt-or-purchase-token",
  "storeTransactionId": "GPA.1234-5678-9012"
}

// Response (basarili)
{
  "data": {
    "id": "subscription-guid",
    "sellerId": "...",
    "subscriptionPlanId": "...",
    "status": 0,             // 0=Active
    "period": 0,             // 0=Monthly
    "platform": 0,           // 0=Apple
    "startedAt": "2026-03-15T10:00:00Z",
    "expiresAt": "2026-04-15T10:00:00Z",
    "autoRenew": true,
    "createdAt": "..."
  }
}
```

**Mobil Ekip (iOS):**
- StoreKit 2 entegrasyonu
- Plan'daki `appleProductIdMonthly` / `appleProductIdYearly` ile urun cagir
- Transaction'dan receipt al, base64 encode et
- `platform: 0` gonder

**Mobil Ekip (Android):**
- Google Play Billing Library v6+
- Plan'daki `googleProductIdMonthly` / `googleProductIdYearly` ile urun cagir
- purchaseToken'i `receipt` olarak gonder
- `platform: 1` gonder

**Web Ekibi:**
- Simdilik Stripe veya baska web odeme yontemi planlanmiyorsa, kullaniciyi mobil uygulamaya yonlendirin
- Ya da `platform: 2` ile web odeme entegrasyonu yapilacaksa ayri tartisilacak

---

### 1.3 Mevcut Abonelik Durumunu Goruntuleme

**Endpoint:** `POST /livestocktrading/SellerSubscriptions/Detail`
**Auth:** Gerekmez (Public - SellerId ile sorgulanir)

```json
// Request
{ "sellerId": "satici-guid" }

// Response (aktif abonelik varsa)
{
  "data": {
    "id": "subscription-guid",
    "sellerId": "...",
    "subscriptionPlanId": "plan-guid",
    "planName": "Pro Plan",
    "planTier": 2,
    "status": 0,                      // 0=Active, 1=Expired, 2=Cancelled, 3=GracePeriod
    "period": 0,
    "platform": 0,
    "startedAt": "2026-03-15T...",
    "expiresAt": "2026-04-15T...",
    "autoRenew": true,
    "maxActiveListings": 50,
    "maxPhotosPerListing": 20,
    "monthlyBoostCredits": 5,
    "hasDetailedAnalytics": true,
    "currentActiveListings": 12,      // su an aktif ilan sayisi
    "remainingListings": 38,          // kalan ilan hakki (-1 = sinirsiz)
    "createdAt": "..."
  }
}

// Response (abonelik yoksa - Free plan)
{
  "data": {
    "status": -1,
    "currentActiveListings": 2,
    "remainingListings": 1,
    "maxActiveListings": 3             // Free plan default
  }
}
```

**UI Gereksinimleri:**
- Satici panelinde "Aboneligim" bolumu
- Mevcut plan adi ve seviyesi
- Kalan ilan hakki progress bar: `currentActiveListings / maxActiveListings`
- Bitis tarihi ve geri sayim
- Otomatik yenileme durumu (acik/kapali switch)
- "Plani Yukselt" butonu
- "Aboneligi Iptal Et" butonu

---

### 1.4 Abonelik Guncelleme (Iptal / Ayarlar)

**Endpoint:** `POST /livestocktrading/SellerSubscriptions/Update`
**Auth:** Gerekli (Seller rolu)

```json
// Otomatik yenileme kapatma
{ "id": "subscription-guid", "autoRenew": false }

// Aboneligi iptal etme
{ "id": "subscription-guid", "status": 2 }
```

> **Not:** Iptal edildiginde `status: 2 (Cancelled)` olur ama abonelik suresi dolana kadar ozellikler kullanilmaya devam eder. Store tarafinda da iptal islemini yapmayi unutmayin.

---

### 1.5 Ilan Limiti Kontrolu

Satici yeni ilan olusturmayi denediginde:

1. Oncelikle `SellerSubscriptions/Detail` ile `remainingListings` kontrol edin
2. `remainingListings == 0` ise:
   - Ilan olusturma formunu engelle
   - "Ilan limitinize ulastiniz. Planınızı yükseltin." mesaji goster
   - Plan yukseltme sayfasina yonlendir
3. `remainingListings == -1` ise: Sinirsiz (Business plan)
4. `remainingListings > 0` ise: Normal akis, opsiyonel olarak "X ilan hakkiniz kaldi" gosterin

**Backend Hata Kodu:** `SellerSubscriptionListingLimitReached` - Products/Create cagirildiginda limit asilirsa bu hata doner

---

## 2. BOOST PAKETLERI (URUN ONE CIKARMA)

### 2.1 Boost Paketlerini Listeleme

**Endpoint:** `POST /livestocktrading/BoostPackages/All`
**Auth:** Gerekmez (Public)

```json
// Request
{ "languageCode": "tr" }

// Response
{
  "data": [
    {
      "id": "paket-guid",
      "name": "Gunluk Boost",
      "description": "Urununu 24 saat boyunca one cikar",
      "durationHours": 24,
      "price": 2.99,
      "currency": "USD",
      "appleProductId": "com.livestock.boost.daily",
      "googleProductId": "boost_daily",
      "boostType": 0,        // 0=Daily, 1=Weekly, 2=Mega
      "boostScore": 10,      // siralama agirligi (yuksek = daha ustte)
      "sortOrder": 1,
      "isActive": true,
      "createdAt": "..."
    },
    {
      "id": "paket-guid-2",
      "name": "Haftalik Boost",
      "description": "7 gun boyunca one cikar",
      "durationHours": 168,
      "price": 14.99,
      "currency": "USD",
      "appleProductId": "com.livestock.boost.weekly",
      "googleProductId": "boost_weekly",
      "boostType": 1,
      "boostScore": 20,
      "sortOrder": 2,
      "isActive": true,
      "createdAt": "..."
    },
    {
      "id": "paket-guid-3",
      "name": "Mega Boost",
      "description": "30 gun boyunca maksimum gorunurluk",
      "durationHours": 720,
      "price": 49.99,
      "currency": "USD",
      "appleProductId": "com.livestock.boost.mega",
      "googleProductId": "boost_mega",
      "boostType": 2,
      "boostScore": 50,
      "sortOrder": 3,
      "isActive": true,
      "createdAt": "..."
    }
  ]
}
```

---

### 2.2 Urun Boost Etme (Satin Alma)

**Endpoint:** `POST /livestocktrading/ProductBoosts/Create`
**Auth:** Gerekli (Seller rolu)

**Akis:**
1. Satici kendi urununde "One Cikar" butonuna tiklar
2. Boost paketi secim popup'i acilir (BoostPackages/All'dan)
3. Paket secilir → IAP akisi baslar
4. Store'dan receipt alinir → backend'e gonderilir:

```json
// Request
{
  "productId": "urun-guid",
  "sellerId": "satici-guid",
  "boostPackageId": "paket-guid",
  "platform": 0,                    // 0=Apple, 1=Google, 2=Web
  "receipt": "base64-receipt",
  "storeTransactionId": "txn-123"
}

// Response
{
  "data": {
    "id": "boost-guid",
    "productId": "urun-guid",
    "sellerId": "satici-guid",
    "boostPackageId": "paket-guid",
    "startedAt": "2026-03-15T10:00:00Z",
    "expiresAt": "2026-03-16T10:00:00Z",  // startedAt + durationHours
    "boostScore": 10,
    "isActive": true,
    "createdAt": "..."
  }
}
```

**Not:** Boost basarili olunca backend otomatik olarak:
- `Product.IsFeatured = true`
- `Product.FeaturedUntil = expiresAt`
- `Product.BoostScore = boostScore`
ayarlar. Urun listeleme siralamasinda boost'lu urunler one cikar.

---

### 2.3 Aktif Boost Gecmisini Goruntuleme

**Endpoint:** `POST /livestocktrading/ProductBoosts/All`
**Auth:** Gerekli (Seller rolu)

```json
// Request
{
  "sellerId": "satici-guid",
  "pageRequest": { "page": 1, "pageSize": 20 }
}

// Response
{
  "data": [
    {
      "id": "boost-guid",
      "productId": "urun-guid",
      "productTitle": "Siyah Angus Boga",
      "boostPackageId": "paket-guid",
      "boostPackageName": "Haftalik Boost",
      "boostType": 1,
      "startedAt": "2026-03-10T10:00:00Z",
      "expiresAt": "2026-03-17T10:00:00Z",
      "boostScore": 20,
      "isActive": true,         // hala devam ediyor mu
      "isExpired": false,        // suresi dolmus mu
      "createdAt": "..."
    }
  ]
}
```

**UI Gereksinimleri:**
- Satici panelinde "Boost'larim" bolumu
- Aktif boost'lar: kalan sure gostergesi (countdown timer)
- Suresi dolan boost'lar: farkli renk/stil (`isExpired: true`)
- Her boost icin urun adi ve paket tipi
- "Yeniden Boost Et" butonu (suresi dolmuslar icin)

---

### 2.4 Urun Listesinde Boost Gosterimi

Products/All ve Products/Search response'larinda boost'lu urunler otomatik olarak **daha ust siralarda** gelir (backend `BoostScore`'a gore siralar).

**Frontend/Mobil tarafinda:**
- Boost'lu urunlere "One Cikan" / "Sponsored" / "Featured" badge'i gosterin
- Ozel arka plan rengi veya cerceve ile vurgulama
- Badge'i nasil anlarsiniz: Products/All response'unda urunun `boostScore > 0` veya siralama seklinden anlayabilirsiniz. Opsiyonel olarak Products/Detail ile `isFeatured` ve `featuredUntil` alanlari mevcuttur.

---

## 3. ENUM REFERANSI

```typescript
// Abonelik
enum SubscriptionTier { Free = 0, Basic = 1, Pro = 2, Business = 3 }
enum SubscriptionStatus { Active = 0, Expired = 1, Cancelled = 2, GracePeriod = 3 }
enum SubscriptionPeriod { Monthly = 0, Yearly = 1 }
enum SubscriptionPlatform { Apple = 0, Google = 1, Web = 2 }
enum SubscriptionTargetType { Seller = 0, Transporter = 1 }

// Boost
enum BoostType { Daily = 0, Weekly = 1, Mega = 2 }

// IAP Islem
enum IAPTransactionType { Subscription = 0, Boost = 1 }
enum IAPTransactionStatus { Pending = 0, Validated = 1, Failed = 2, Refunded = 3 }
```

---

## 4. HATA KODLARI

Frontend'de yakalanmasi gereken monetizasyon hata kodlari:

| Hata Kodu | Aciklama | UI Aksiyonu |
|-----------|----------|-------------|
| `SubscriptionPlanNotFound` | Plan bulunamadi | Planlari yeniden yukle |
| `SubscriptionPlanNotActive` | Plan aktif degil | "Bu plan artik mevcut degil" |
| `SellerSubscriptionAlreadyActive` | Zaten aktif abonelik var | Mevcut plani goster |
| `SellerSubscriptionExpired` | Abonelik suresi dolmus | Yenileme sayfasina yonlendir |
| `SellerSubscriptionSellerRequired` | SellerId eksik | - |
| `SellerSubscriptionPlanRequired` | PlanId eksik | - |
| `SellerSubscriptionReceiptRequired` | Receipt eksik | IAP akisini tekrarla |
| `SellerSubscriptionReceiptInvalid` | Receipt gecersiz | "Satin alma dogrulanamadi" |
| `SellerSubscriptionListingLimitReached` | Ilan limiti doldu | Plan yukseltme sayfasina yonlendir |
| `BoostPackageNotFound` | Boost paketi bulunamadi | Paketleri yeniden yukle |
| `BoostPackageNotActive` | Boost paketi aktif degil | "Bu paket artik mevcut degil" |
| `ProductBoostAlreadyActive` | Urun zaten boost'lu | Mevcut boost bilgisini goster |
| `ProductBoostProductRequired` | ProductId eksik | - |
| `ProductBoostPackageRequired` | PackageId eksik | - |
| `ProductBoostReceiptRequired` | Receipt eksik | IAP akisini tekrarla |
| `ProductBoostReceiptInvalid` | Receipt gecersiz | "Satin alma dogrulanamadi" |

---

## 5. IS DAGITIMI & ONCELIK

### iOS Ekibi
| Oncelik | Gorev | Tahmini Sure |
|---------|-------|-------------|
| P0 | StoreKit 2 entegrasyonu (abonelik + consumable IAP) | - |
| P0 | Abonelik planlari sayfasi (karsilastirmali tablo) | - |
| P0 | Abonelik satin alma akisi (plan sec → IAP → receipt gonder) | - |
| P1 | Mevcut abonelik durumu gosterimi | - |
| P1 | Boost paketi satin alma akisi | - |
| P1 | Ilan limiti kontrolu (Products/Create oncesi) | - |
| P2 | Boost gecmisi sayfasi | - |
| P2 | Abonelik iptal / otomatik yenileme ayarlari | - |

### Android Ekibi
| Oncelik | Gorev | Tahmini Sure |
|---------|-------|-------------|
| P0 | Google Play Billing Library v6+ entegrasyonu | - |
| P0 | Abonelik planlari sayfasi | - |
| P0 | Abonelik satin alma akisi | - |
| P1 | Mevcut abonelik durumu gosterimi | - |
| P1 | Boost paketi satin alma akisi | - |
| P1 | Ilan limiti kontrolu | - |
| P2 | Boost gecmisi sayfasi | - |
| P2 | Abonelik iptal / otomatik yenileme ayarlari | - |

### Web Ekibi
| Oncelik | Gorev | Tahmini Sure |
|---------|-------|-------------|
| P0 | Abonelik planlari sayfasi (karsilastirmali tablo + Aylik/Yillik toggle) | - |
| P0 | Mevcut abonelik durumu gosterimi (satici paneli) | - |
| P1 | Ilan limiti kontrolu & uyari gosterimi | - |
| P1 | Boost paketi secim popup'i + satin alma akisi | - |
| P1 | Urun listesinde "One Cikan" badge'i | - |
| P2 | Boost gecmisi sayfasi | - |
| P2 | Web odeme entegrasyonu (Stripe vb.) veya mobil yonlendirme | - |

---

## 6. TEST SENARYOLARI

Backend `.http` dosyalari: `_doc/Http/BusinessModules/LivestockTrading/`

### Temel Akis Testi
1. `SubscriptionPlans/All` cagir → planlari al
2. Bir plan sec → IAP ile satin al → `SellerSubscriptions/Create` cagir
3. `SellerSubscriptions/Detail` ile abonelik durumunu dogrula
4. `remainingListings` kontrolu yap
5. `BoostPackages/All` cagir → paketleri al
6. Bir paket sec → IAP ile satin al → `ProductBoosts/Create` cagir
7. `ProductBoosts/All` ile boost gecmisini kontrol et
8. Products/All'da boost'lu urunun ustte geldigini dogrula

### Edge Case'ler
- Free plan ile 4. ilan olusturmaya calis → `ListingLimitReached` hatasi
- Zaten aktif aboneligi varken yeni abonelik olustur → `AlreadyActive` hatasi
- Zaten boost'lu urune tekrar boost → `AlreadyActive` hatasi
- Suresi dolmus abonelik ile ilan olustur → limit kontrolu
- Gecersiz receipt gonder → `ReceiptInvalid` hatasi

---

## NOTLAR

- Tum endpoint'ler `POST` method'u kullanir
- Base URL: `https://{domain}/livestocktrading/`
- Auth gereken endpoint'ler: `Authorization: Bearer {jwt-token}` header'i
- Content-Type: `application/json`
- Fiyatlar USD cinsindendir, kullanicinin para birimine cevirme icin `Currencies/Convert` endpoint'i mevcut
- Coklu dil: `languageCode` parametresi ile plan/paket isim ve aciklamalari otomatik cevrilir
