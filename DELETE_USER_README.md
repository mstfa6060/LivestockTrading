# Kullanıcı Silme SQL Scriptleri

Bu klasörde kullanıcı ve tüm ilişkili verilerini silmek için 2 farklı SQL scripti bulunmaktadır.

## 📋 Dosyalar

### 1. `delete_user_script.sql` - **HARD DELETE (Kalıcı Silme)**
Kullanıcıyı ve tüm ilişkili verilerini **veritabanından tamamen siler**. Bu işlem geri alınamaz!

### 2. `soft_delete_user_script.sql` - **SOFT DELETE (Yumuşak Silme)** ⭐ ÖNERİLEN
Kullanıcıyı ve tüm ilişkili verilerini `IsDeleted=1` yaparak **işaretler**. Veriler veritabanında kalır, gerektiğinde geri getirilebilir.

---

## 🚀 Kullanım

### Adım 1: Scripti Açın
İhtiyacınıza göre uygun scripti SQL Server Management Studio (SSMS) veya Azure Data Studio'da açın.

### Adım 2: User ID'yi Değiştirin
Scriptin başındaki `@userId` değişkenini silmek istediğiniz kullanıcının GUID'i ile değiştirin:

```sql
DECLARE @userId UNIQUEIDENTIFIER = 'BURAYA-KULLANICI-ID-YAZIN';
```

**Örnek:**
```sql
DECLARE @userId UNIQUEIDENTIFIER = 'A1B2C3D4-E5F6-7890-ABCD-EF1234567890';
```

### Adım 3: Scripti Çalıştırın
- SQL scriptini çalıştırın (F5 veya Execute)
- Script otomatik olarak bir TRANSACTION içinde çalışır
- Hata durumunda tüm işlemler geri alınır (ROLLBACK)

---

## 📊 Silinen/İşaretlenen Tablolar

### Hirovo Modülü
-  `HirovoJobApplications` (WorkerId)
-  `HirovoJobs` (HirovoEmployerId)
-  `HirovoSubscriptions` (UserId)
-  `HirovoFileAccessCodes` (GeneratorUserId)
-  `HirovoDocumentRoots` (CreatorUserId, ManualCreatorUserId)

### AnimalMarket Modülü (⚠️ `AnimalMarket_` prefix'i ile)
-  `AnimalMarket_FarmUsers` (UserId)
-  `AnimalMarket_VeterinaryApprovals` (VeterinarianId)
-  `AnimalMarket_DeliveryOrders` (CarrierId, BuyerId)
-  `AnimalMarket_AnimalBids` (BidderId)
-  `AnimalMarket_AnimalOffers` (BuyerId)
-  `AnimalMarket_EscrowTransactions` (BuyerId, SellerId)
-  `AnimalMarket_PaymentTransactions` (UserId - varsa)
-  `AnimalMarket_Animals` (SellerId)
-  `AnimalMarket_Farms` (OwnerId)

### Profil Tabloları (⚠️ `AnimalMarket_` prefix'i ile)
-  `AnimalMarket_VeterinarianProfiles` (UserId)
-  `AnimalMarket_VeterinarianProfileServiceAreas` (ilişkili)
-  `AnimalMarket_VeterinarianProfileCertifications` (ilişkili)
-  `AnimalMarket_TransporterProfiles` (UserId)
-  `AnimalMarket_AnimalOwnerProfiles` (UserId)

### Ortak Tablolar
-  `AppAuditLogs` (UserId - varsa)
-  `UserRoles` (UserId - varsa)
-  `AppUsers` (Ana kullanıcı tablosu)

---

## ⚠️ ÖNEMLİ UYARILAR

### ⚠️ Tablo İsimleri
- **Hirovo modülü**: Direkt tablo isimleri (`HirovoJobs`, `HirovoJobApplications`)
- **AnimalMarket modülü**: `AnimalMarket_` prefix'i ile (`AnimalMarket_Animals`, `AnimalMarket_Farms`)
- **IAM/Common**: `App` prefix'i ile (`AppUsers`, `AppCompanies`, `AppRoles`)

### ⚠️ Eksik Tablolar
Script her tablo için `IF OBJECT_ID()` kontrolü yapar. Eğer tablo veritabanında yoksa:
-  Hata vermez, "atlanıyor" mesajı verir
-  İşlem devam eder
- ℹ️ Bu normal: Hirovo veya AnimalMarket modülleri farklı veritabanlarında olabilir

### Hard Delete Kullanıyorsanız:
- ❌ **GERİ ALINAMAZ!** Silinen veriler geri getirilemez
- 💾 İşlem öncesi **mutlaka yedek alın**
- 🔒 **Üretim ortamında çok dikkatli kullanın**
- 📋 Silme işlemi öncesi kullanıcı verilerini export edin

### Soft Delete Kullanıyorsanız (ÖNERİLEN):
-  Veriler veritabanında kalır
-  Gerekirse geri getirilebilir (`IsDeleted=0` yapılarak)
-  Üretim ortamı için daha güvenli
- ⚠️ Disk alanı tüketmeye devam eder

---

## 🔍 Kullanıcı ID'sini Bulma

Kullanıcı ID'sini bulmak için:

```sql
-- Email ile bulma
SELECT Id, UserName, Email, FirstName, Surname
FROM AppUsers
WHERE Email = 'kullanici@example.com'
AND IsDeleted = 0;

-- İsim ile bulma
SELECT Id, UserName, Email, FirstName, Surname
FROM AppUsers
WHERE FirstName LIKE '%Ali%'
AND IsDeleted = 0;

-- Username ile bulma
SELECT Id, UserName, Email, FirstName, Surname
FROM AppUsers
WHERE UserName = 'username'
AND IsDeleted = 0;
```

---

## 🔄 Soft Delete'i Geri Alma

Soft delete edilmiş bir kullanıcıyı geri getirmek için:

```sql
DECLARE @userId UNIQUEIDENTIFIER = 'KULLANICI_ID_BURAYA';

BEGIN TRANSACTION;

-- Kullanıcıyı geri getir
UPDATE AppUsers
SET IsDeleted = 0, DeletedAt = NULL, IsActive = 1
WHERE Id = @userId;

-- İlişkili tabloları da geri getir
UPDATE HirovoJobApplications SET IsDeleted = 0, DeletedAt = NULL WHERE WorkerId = @userId;
UPDATE HirovoJobs SET IsDeleted = 0, DeletedAt = NULL WHERE HirovoEmployerId = @userId;
UPDATE Animals SET IsDeleted = 0, DeletedAt = NULL WHERE SellerId = @userId;
-- ... diğer tablolar için de tekrarlayın

COMMIT TRANSACTION;
```

---

## 📝 Log Çıktısı

Script çalıştırıldığında aşağıdaki gibi bir çıktı alırsınız:

```
================================================
Kullanıcı silme işlemi başlıyor...
User ID: A1B2C3D4-E5F6-7890-ABCD-EF1234567890
================================================
Hirovo modülü temizleniyor...
  - HirovoJobApplications silindi (WorkerId)
  - HirovoJobs silindi (HirovoEmployerId)
  - HirovoSubscriptions silindi
  ...
================================================
BAŞARILI: Kullanıcı ve tüm ilişkili veriler silindi!
User ID: A1B2C3D4-E5F6-7890-ABCD-EF1234567890
================================================
```

---

## 🆘 Hata Durumunda

Eğer script hata verirse:
1.  Tüm değişiklikler otomatik olarak **geri alınır** (ROLLBACK)
2.  Hata mesajı ve satır numarası gösterilir
3.  Veritabanı tutarlılığı korunur

**Yaygın hatalar:**
- **Foreign Key Constraint:** Tablolar arasında eksik ilişki varsa CASCADE DELETE ayarlarını kontrol edin
- **Invalid GUID:** User ID formatının doğru olduğundan emin olun
- **Permission Denied:** Yeterli veritabanı yetkilerine sahip olduğunuzdan emin olun

---

## 💡 Öneriler

1. **Test Ortamında Deneyin:** İlk kullanımda mutlaka test ortamında deneyin
2. **Soft Delete Kullanın:** Üretim ortamında soft delete kullanmanız önerilir
3. **Yedek Alın:** Hard delete kullanacaksanız mutlaka yedek alın
4. **Logları İnceleyin:** Script çıktısını mutlaka inceleyin
5. **CASCADE DELETE:** Veritabanında CASCADE DELETE ayarları varsa dikkatli olun

---

## 📞 Destek

Sorun yaşarsanız:
- Script çıktısını kaydedin
- Hata mesajını not edin
- Veritabanı yedeğinizi kontrol edin
