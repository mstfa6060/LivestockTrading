# AnimalMarket Module - Complete API Documentation

Bu dokümantasyon AnimalMarket modülü için tüm endpoint'leri içerir.

## API Base URL
```
http://localhost:5000
```

## Authentication
Tüm endpoint'ler için JWT token gereklidir (özel belirtilmedikçe).

## HTTP Test Files

### 1. Animals.http
Hayvan yönetimi endpoint'leri:
-  Animal Create - Yeni hayvan ekleme
-  Animal Update - Hayvan bilgilerini güncelleme  
-  Animal Delete - Hayvan silme (soft delete)
-  Animal Filter - Gelişmiş arama ve filtreleme
-  Animal Detail - Hayvan detay bilgileri
-  Animal List (All) - Tüm hayvanları listeleme
-  Animal Status Update - Durum güncelleme
-  Animal Price Update - Fiyat güncelleme
-  Animals By Farm - Çiftlik hayvanları
-  Animals By Seller - Satıcı hayvanları
-  Animal Auction Start - Açık artırma başlatma
-  Animal Auction End - Açık artırma sonlandırma
-  Animal Statistics - İstatistik bilgileri

### 2. Bids.http
Teklif yönetimi endpoint'leri:
-  Bid Create - Yeni teklif verme
-  Bid Update - Teklif güncelleme
-  Bid Cancel - Teklif iptal etme
-  Bid Accept - Teklif kabul etme
-  Bid Reject - Teklif reddetme
-  Bids By Animal - Hayvana göre teklifler
-  Bids By User - Kullanıcı teklifleri
-  Bids Received - Alınan teklifler
-  Bid History - Teklif geçmişi
-  Auto Bid Setup - Otomatik teklif kurulum
-  Auto Bid Cancel - Otomatik teklif iptal
-  Bid Statistics - Teklif istatistikleri

### 3. Offers.http
Teklif yönetimi endpoint'leri (Negotiable sales için):
-  Offer Create - Yeni teklif oluşturma
-  Offer Update - Teklif güncelleme
-  Offer Cancel - Teklif iptal etme
-  Offer Accept - Teklif kabul etme
-  Offer Reject - Teklif reddetme
-  Offer Counter - Karşı teklif verme
-  Offers By Animal - Hayvana göre teklifler
-  Offers By User - Kullanıcı teklifleri
-  Offers Received - Alınan teklifler
-  Offer Detail - Teklif detayları
-  Offer History - Teklif geçmişi
-  Offer Statistics - Teklif istatistikleri

### 4. Farms.http
Çiftlik yönetimi endpoint'leri:
-  Farm Create - Yeni çiftlik oluşturma
-  Farm Update - Çiftlik bilgilerini güncelleme
-  Farm Detail - Çiftlik detay bilgileri
-  Farm Delete - Çiftlik silme
-  Farms By Owner - Sahibine göre çiftlikler
-  Farm Search - Çiftlik arama
-  Farm Statistics - Çiftlik istatistikleri
-  Farm Verify - Çiftlik doğrulama
-  Farm Animals Count - Çiftlik hayvan sayısı

### 5. AnimalPhotos.http
Hayvan fotoğraf yönetimi endpoint'leri:
-  Animal Photo Upload - Fotoğraf yükleme
-  Animal Photo Update - Fotoğraf güncelleme
-  Animal Photo Delete - Fotoğraf silme
-  Animal Photos List - Fotoğraf listeleme
-  Animal Photo Set Primary - Ana fotoğraf belirleme
-  Animal Photos Reorder - Fotoğraf sıralama
-  Animal Photo Detail - Fotoğraf detayları

### 6. VeterinaryDocuments.http
Veteriner belge yönetimi endpoint'leri:
-  Veterinary Document Upload - Belge yükleme
-  Veterinary Document Update - Belge güncelleme
-  Veterinary Document Delete - Belge silme
-  Veterinary Documents By Animal - Hayvan belgeleri
-  Veterinary Document Detail - Belge detayları
-  Veterinary Documents By Farm - Çiftlik belgeleri
-  Veterinary Documents Expiring Soon - Süresi yaklaşan belgeler
-  Veterinary Document Verify - Belge doğrulama

### 7. Search.http
Arama ve keşif endpoint'leri:
-  Search Animals - Hayvan arama
-  Search Animals By Location - Lokasyon bazlı arama
-  Search Farms - Çiftlik arama
-  Search Suggestions - Arama önerileri
-  Search Popular Terms - Popüler arama terimleri
-  Search History - Arama geçmişi
-  Search Save Query - Arama kaydı

### 8. UserProfile.http
Kullanıcı profil ve tercih endpoint'leri:
-  User Watchlist Add Animal - İzleme listesi ekleme
-  User Watchlist Remove Animal - İzleme listesi çıkarma
-  User Watchlist Get - İzleme listesi getirme
-  User Favorites Add Farm - Favori çiftlik ekleme
-  User Favorites Remove Farm - Favori çiftlik çıkarma
-  User Favorites Get Farms - Favori çiftlikler
-  User Profile Update Preferences - Tercih güncelleme
-  User Activity History - Aktivite geçmişi
-  User Dashboard Stats - Dashboard istatistikleri

### 9. Notifications.http
Bildirim yönetimi endpoint'leri:
-  Notification Send - Bildirim gönderme
-  Notifications Get User - Kullanıcı bildirimleri
-  Notification Mark Read - Okundu işaretleme
-  Notifications Mark All Read - Tümünü okundu işaretle
-  Notification Delete - Bildirim silme
-  Notification Settings Update - Bildirim ayarları
-  Notification Settings Get - Bildirim ayarlarını getir
-  Notifications Send Bulk - Toplu bildirim

### 10. Reports.http
Raporlama ve analiz endpoint'leri:
-  Reports Sales Summary - Satış özeti
-  Reports Market Analysis - Pazar analizi
-  Reports Price Trends - Fiyat trendleri
-  Reports User Activity - Kullanıcı aktivitesi
-  Reports Farm Performance - Çiftlik performansı
-  Reports Auction Analysis - Açık artırma analizi
-  Reports Popular Animals - Popüler hayvanlar
-  Reports Revenue Summary - Gelir özeti
-  Reports Export Data - Veri dışa aktarma

### 11. deliveryOrders.http
Teslimat yönetimi endpoint'leri:
-  Delivery Orders Create - Yeni teslimat oluşturma
-  Delivery Orders Update - Teslimat güncelleme
-  Delivery Orders Delete - Teslimat silme
-  DeliveryOrders All - Tüm teslimatlar
-  Delivery Orders Detail - Teslimat detayları
-  Delivery Orders Pick - Teslimat seçimi
-  **Update Delivery Location** - 🆕 GPS konum güncelleme (Nakliyeci)
-  **Get Delivery Tracking** - 🆕 Teslimat takibi (Nakliyeci/Alıcı/Satıcı)

### 12. transporterProfiles.http
Nakliyeci profil yönetimi endpoint'leri:
-  Transporter Profiles Create - Yeni nakliyeci profili
-  Transporter Profiles Update - Profil güncelleme
-  Transporter Profiles Delete - Profil silme
-  TransporterProfiles All - Tüm nakliyeciler
-  TransporterProfiles Detail - Nakliyeci detayları
-  TransporterProfiles Pick - Nakliyeci seçimi
-  **Update Transporter Availability** - 🆕 Müsaitlik güncelleme + GPS konum
-  **Get Transporters On Map** - 🆕 Haritada nakliyecileri göster (filtrelemeli)

## GPS Tracking System 🗺️

### Özellikler
- 📍 **Gerçek Zamanlı Konum Takibi**: Nakliyecilerin anlık konumları
- 🧭 **ETA Hesaplama**: Haversine formülü ile tahmini varış süresi
- 🚚 **Teslimat Takibi**: Alıcı ve satıcılar teslimatı izleyebilir
- 🗺️ **Harita Entegrasyonu**: Müsait nakliyecileri haritada gösterme
- 🎯 **Akıllı Filtreleme**: Kapasite, rating, araç tipi bazında filtreleme

### Request Örnekleri

#### 1. Konum Güncelleme (Nakliyeci)
```http
POST /DeliveryOrders/UpdateLocation
{
  "deliveryOrderId": "guid",
  "latitude": 39.9334,
  "longitude": 32.8597,
  "timestamp": "2025-10-24T14:30:00Z"
}
```

#### 2. Teslimat Takibi
```http
POST /DeliveryOrders/GetTracking
{
  "deliveryOrderId": "guid"
}
```

**Response:**
```json
{
  "deliveryOrderId": "guid",
  "status": "InTransit",
  "animal": { "id": "...", "name": "Simental 001" },
  "transporter": { "name": "Ahmet", "phoneNumber": "+90..." },
  "route": {
    "pickup": { "lat": 37.8746, "lng": 32.4932, "address": "Konya, Selçuklu" },
    "dropoff": { "lat": 39.9334, "lng": 32.8597, "address": "Ankara, Yenimahalle" },
    "currentLocation": { "lat": 39.0000, "lng": 32.5000 }
  },
  "progress": {
    "distanceTraveled": 125.5,
    "totalDistance": 250.0,
    "percentageComplete": 50.2,
    "estimatedArrival": "2025-10-24T16:45:00Z"
  }
}
```

#### 3. Haritada Nakliyeciler
```http
POST /TransporterProfiles/GetOnMap
{
  "bounds": {
    "northEast": { "lat": 41.0082, "lng": 29.0100 },
    "southWest": { "lat": 38.0000, "lng": 26.0000 }
  },
  "filters": {
    "onlyAvailable": true,
    "minCapacity": 10,
    "vehicleTypes": ["Kamyon"],
    "minRating": 4.0
  }
}
```

## Endpoint Kategorileri Özeti

### Core Entities (Temel Varlıklar)
- **Animals**: 13 endpoint - CRUD + advanced features
- **Farms**: 9 endpoint - CRUD + management features
- **Bids**: 12 endpoint - Auction system
- **Offers**: 12 endpoint - Negotiation system

### Supporting Entities (Destekleyici Varlıklar)
- **AnimalPhotos**: 7 endpoint - Media management
- **VeterinaryDocuments**: 8 endpoint - Document management
- **DeliveryOrders**: 8 endpoint - 🆕 Delivery management + GPS tracking
- **TransporterProfiles**: 8 endpoint - 🆕 Transporter management + GPS

### Platform Features (Platform Özellikleri)
- **Search**: 7 endpoint - Search & discovery
- **UserProfile**: 9 endpoint - User preferences & activity
- **Notifications**: 8 endpoint - Communication system
- **Reports**: 9 endpoint - Analytics & insights

## Toplam Endpoint Sayısı: 110 (🆕 +16 GPS tracking)

## Test Etme
Her HTTP dosyasını VS Code REST Client extension ile test edebilirsiniz.

## Güvenlik
- Tüm endpoint'ler JWT authentication gerektirir
- Multi-tenancy Farm bazında uygulanır
- Rate limiting ve input validation aktiftir

## Response Format
Tüm endpoint'ler standardize edilmiş ArfBlocks response format kullanır:
```json
{
  "success": true,
  "data": {...},
  "errors": [],
  "pagination": {...}
}
```
