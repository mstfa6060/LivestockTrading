# 🚀 ROL SİSTEMİ - SEÇENEK 2 İMPLEMENTASYON ROADMAP

## 📋 Proje: UserRole Tablosuna ModuleId Ekleme

**Başlangıç Tarihi**: 16 Kasım 2025  
**Durum**: 🟡 Devam Ediyor  
**Tahmini Süre**: 4-6 saat

---

##  ADIMLAR

### BACKEND - Database

- [x] **ADIM 1**: Migration Dosyası Oluştur 
  - Dosya: `Jobs/RelationalDB/MigrationJob/Migrations/{{TIMESTAMP}}_AddModuleIdToUserRole.cs`
  - Durum: 🟢 Tamamlandı
  - Süre: ~15 dakika

- [x] **ADIM 2**: UserRole Entity Güncelle 
  - Dosya: `Common/Definitions/Common.Definitions.Domain/RelationalEntities/User-Roles/UserRole.cs`
  - Durum: 🟢 Tamamlandı
  - Not: `ModuleId` property ve `Module` navigation property eklendi
  - Süre: ~5 dakika

- [ ] **ADIM 3**: RoleRelations Seed Data Güncelle
  - Dosya: `Common/Definitions/Common.Definitions.Infrastructure/RelationalDB/Relations/RoleRelations.cs`
  - Durum: ⏳ Bekliyor
  - Not: Mevcut UserRole seed data'larına ModuleId eklenmeli
  - Süre: ~30 dakika

- [ ] **ADIM 4**: Migration'ı Çalıştır
  - Komut: `dotnet ef migrations add AddModuleIdToUserRole` (zaten yapıldı )
  - Komut: `dotnet run development` (yapılacak ⏳)
  - Durum: ⏳ Bekliyor
  - Not: Migration çalıştırılıp database'e ModuleId kolonu eklenmeli
  - Süre: ~10 dakika

---

### BACKEND - Services

- [x] **ADIM 5**: Login Handler DataAccess Güncelle 
  - Dosya: `BaseModules/IAM/BaseModules.IAM.Application/RequestHandlers/Auth/Commands/Login/DataAccess.cs`
  - Durum: 🟢 Tamamlandı
  - Not: `GetUserRolesWithModule` metodu eklendi
  - Süre: ~20 dakika

- [x] **ADIM 6**: Login Handler Güncelle 
  - Dosya: `BaseModules/IAM/BaseModules.IAM.Application/RequestHandlers/Auth/Commands/Login/Handler.cs`
  - Durum: 🟢 Tamamlandı
  - Not: Rolleri `"ModuleName.RoleName"` formatında JWT'ye gönderiyor
  - Süre: ~30 dakika

- [x] **ADIM 7**: JwtService Interface Güncelle 
  - Dosya: `Common/Services/Common.Services.Auth/JsonWebToken/IJwtService.cs`
  - Durum: 🟢 Tamamlandı
  - Not: `List<string> roles = null` parametresi eklendi
  - Süre: ~5 dakika

- [x] **ADIM 8**: JwtService Implementation Güncelle 
  - Dosya: `Common/Services/Common.Services.Auth/JsonWebToken/JwtService.cs`
  - Durum: 🟢 Tamamlandı
  - Not: Rolleri JWT claims'e `"role"` type ile ekliyor
  - Süre: ~20 dakika

- [x] **ADIM 9**: RefreshToken Handler Güncelle 
  - Dosya: `BaseModules/IAM/BaseModules.IAM.Application/RequestHandlers/Auth/Commands/RefreshToken/Handler.cs`
  - Durum: 🟢 Tamamlandı
  - Not: `GetUserRolesWithModule` ile rolleri alıp JWT'ye ekliyor
  - Süre: ~15 dakika

- [x] **ADIM 10**: CurrentUserService Genişlet 
  - Dosya: `Common/Services/Common.Services.Auth/CurrentUser/CurrentUserService.cs`
  - Durum: 🟢 Tamamlandı
  - Not: `GetUserRoles()`, `GetUserRolesForModule()`, `HasRoleInModule()` metodları eklendi
  - Süre: ~30 dakika

---

### BACKEND - AnimalMarket

- [x] **ADIM 11**: AssignInitialRole Endpoint 
  - Dosya: `BusinessModules/AnimalMarket/BusinessModules.AnimalMarket.Application/RequestHandlers/UserRoles/Commands/AssignInitialRole/`
  - Durum: 🟢 Tamamlandı
  - Not: Handler, DataAccess, Mapper, RequestModel, ResponseModel, IDataAccess tamamlandı
  - Kullanım: Kullanıcının ilk rol seçimi (Farmer/Veterinarian/Transporter)
  - Süre: ~45 dakika

---

### FRONTEND - Mobil Uygulama

- [ ] **ADIM 12**: AssignInitialRole API Tanımı
  - Dosya: `common/animalmarket-api/src/endpoints/userRoles.ts`
  - Durum: ⏳ Bekliyor
  - Not: TypeScript tip tanımları ve API call fonksiyonu eklenecek
  - Süre: ~15 dakika

- [ ] **ADIM 13**: useRole Hook Oluştur
  - Dosya: `src/hooks/useRole.ts`
  - Durum: ⏳ Bekliyor
  - Not: `hasRoleInModule()`, `isFarmer()`, `isVeterinarian()` gibi helper metodlar
  - Süre: ~20 dakika

- [ ] **ADIM 14**: RoleSelectionScreen Oluştur
  - Dosya: `src/screens/Auth/RoleSelectionScreen.tsx`
  - Durum: ⏳ Bekliyor
  - Not: Kullanıcının rol seçimi yapacağı ekran (Farmer/Veterinarian/Transporter)
  - Süre: ~40 dakika

- [ ] **ADIM 15**: AppNavigator Güncelle
  - Dosya: `src/navigation/AppNavigator.tsx`
  - Durum: ⏳ Bekliyor
  - Not: Login sonrası rol kontrolü, rol yoksa RoleSelectionScreen'e yönlendir
  - Süre: ~20 dakika

---

### TEST & VERIFICATION

- [ ] **ADIM 16**: Backend Test
  - SQL ile rol kontrolü
  - Postman ile login test
  - JWT decode test (jwt.io)
  - Durum: ⏳ Bekliyor
  - Süre: ~30 dakika

- [ ] **ADIM 17**: AssignInitialRole Endpoint Test
  - Postman ile rol atama testi
  - JWT refresh testi
  - Durum: ⏳ Bekliyor
  - Süre: ~20 dakika

- [ ] **ADIM 18**: Frontend Test
  - Login sonrası rol kontrolü
  - RoleSelectionScreen akışı
  - Rol bazlı navigasyon
  - Durum: ⏳ Bekliyor
  - Süre: ~30 dakika

- [ ] **ADIM 19**: End-to-End Test
  - Farklı modüllerde farklı roller
  - Permission kontrolü
  - Rol değiştirme senaryosu
  - Durum: ⏳ Bekliyor
  - Süre: ~30 dakika

---

## 📊 İLERLEME
```
Toplam Adım: 19
Tamamlanan: 11
Kalan: 8
İlerleme: [███████████         ] 58%
```

### Backend İlerleme
```
Toplam: 11
Tamamlanan: 9
İlerleme: [█████████████████   ] 82%
```

### Frontend İlerleme  
```
Toplam: 4
Tamamlanan: 0
İlerleme: [                    ] 0%
```

### Test İlerleme
```
Toplam: 4
Tamamlanan: 0
İlerleme: [                    ] 0%
```

---

## 🔍 ŞU ANDA YAPILACAK

**Sıradaki Adım**: ADIM 4 - Migration'ı Çalıştır

**Detaylar**:
- Migration dosyası zaten oluşturulmuş 
- Şimdi migration'ı database'e uygulayacağız
- UserRole tablosuna ModuleId kolonu eklenecek
- Mevcut veriler için ModuleId default değeri atanacak

**Komut**:
```bash
cd /d/Projects/Maden/backend/Jobs/RelationalDB/MigrationJob
dotnet run development
```

**Alternatif Sıra** (Migration çalıştırılmışsa):
1. **ADIM 12**: AssignInitialRole API tanımı
2. **ADIM 13**: useRole Hook
3. **ADIM 14**: RoleSelectionScreen
4. **ADIM 15**: AppNavigator güncelle

---

## 📝 NOTLAR

### Önemli Kararlar
-  Seçenek 2: UserRole tablosuna ModuleId ekleme
-  Rol isimleri prefix olmadan kalacak (Admin, User, Veterinarian vs.)
-  JWT'de roller "ModuleName.RoleName" formatında (AnimalMarket.Admin)
-  Mevcut veriler migration sırasında AnimalMarket modülüne atanacak
-  AssignInitialRole endpoint'i eklendi (Farmer/Veterinarian/Transporter)

### Tamamlanan İşler
-  UserRole entity'sine ModuleId eklendi
-  Migration dosyası oluşturuldu
-  Login Handler rolleri JWT'ye ekliyor
-  RefreshToken Handler rolleri JWT'ye ekliyor
-  JwtService rolleri claim olarak ekliyor
-  CurrentUserService rolleri parse ediyor
-  AssignInitialRole endpoint'i hazır

### Kalan İşler
- ⏳ Migration çalıştır (database'e uygula)
- ⏳ RoleRelations seed data güncellemesi
- ⏳ Mobil uygulama geliştirmeleri
- ⏳ Test senaryoları

### Riskler
- ⚠️ Migration çalıştırılmadan mobil uygulama test edilemez
- ⚠️ Mevcut UserRole kayıtları için ModuleId ataması yapılmalı
- ⚠️ JWT boyutu artabilir (çok fazla rol varsa)

### Geri Dönüş Planı
- Migration geri alınabilir (Down metodu var)
- Önceki commit'e dönülebilir
- Database backup alınmalı (önemli!)

---

## 🎯 BAŞARI KRİTERLERİ

Proje başarılı sayılacak kriterleri:

-  Aynı kullanıcı farklı modüllerde farklı rollere sahip olabilmeli
-  JWT'de roller modül bazlı görünmeli
- ⏳ Frontend menüler doğru render olmalı
- ⏳ Backend authorization çalışmalı
- ⏳ Mevcut kullanıcılar etkilenmemeli
- ⏳ Yeni kullanıcılar rol seçebilmeli

---

## 🚦 DURUM LEGENDERİ

- 🔴 Başlanmadı
- 🟡 Devam Ediyor
- 🟢 Tamamlandı
- ⏳ Bekliyor
- ⚠️ Sorun Var
-  Test Edildi

---

## 📞 YARDIM

Herhangi bir adımda takılırsanız:
1. ROL_SISTEMI_ROADMAP.md dosyasını kontrol edin
2. İlgili adımın detaylarını okuyun
3. Hata mesajını paylaşın
4. Knowledge'da ilgili kodları arayın

---

## 🔗 İLGİLİ DOSYALAR

**Backend:**
- `Common/Definitions/Common.Definitions.Domain/RelationalEntities/User-Roles/UserRole.cs`
- `BaseModules/IAM/BaseModules.IAM.Application/RequestHandlers/Auth/Commands/Login/Handler.cs`
- `Common/Services/Common.Services.Auth/JsonWebToken/JwtService.cs`
- `BusinessModules/AnimalMarket/BusinessModules.AnimalMarket.Application/RequestHandlers/UserRoles/Commands/AssignInitialRole/`

**Frontend:**
- `src/hooks/useRole.ts` (yapılacak)
- `src/screens/Auth/RoleSelectionScreen.tsx` (yapılacak)
- `src/navigation/AppNavigator.tsx` (güncellenecek)

---

**SON GÜNCELLEME**: 16 Kasım 2025, 23:45  
**GÜNCELLEYEN**: Claude & Mustafa  
**BACKEND İLERLEME**: %82  
**FRONTEND İLERLEME**: %0  
**GENEL İLERLEME**: %58