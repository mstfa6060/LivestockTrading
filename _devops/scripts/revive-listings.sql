BEGIN TRANSACTION;

DECLARE @now DATETIME2 = GETUTCDATE();
DECLARE @expires DATETIME2 = DATEADD(DAY, 60, GETUTCDATE());

-- Seller IDs
DECLARE @s1 UNIQUEIDENTIFIER = 'E4ADB651-9AF2-4DD7-86F8-DED18E3D5577'; -- Yilmaz Hayvancilik
DECLARE @s2 UNIQUEIDENTIFIER = '57EF0F5B-86EE-43E8-8546-402B671D39EE'; -- Ahmet Test
DECLARE @s3 UNIQUEIDENTIFIER = '180A26F4-0B6F-48DF-BA79-691D0E221F42'; -- Mustafa Ocak

-- Location IDs
DECLARE @locIst UNIQUEIDENTIFIER = 'C0E1253E-A1E4-4868-A7F6-BB33169EBF72';  -- Istanbul
DECLARE @locAnk UNIQUEIDENTIFIER = '930F3267-BD23-4508-B28E-0F9601E3D14B';  -- Ankara
DECLARE @locBol UNIQUEIDENTIFIER = 'FBD10477-FDA1-4D0E-A379-7D453BE7F0E4';  -- Bolu
DECLARE @locPen UNIQUEIDENTIFIER = 'AD53775B-9794-492D-9EF9-CFF9DF333C8D';  -- Pendik
DECLARE @locUmr UNIQUEIDENTIFIER = '4C8B4DB3-3504-4417-A570-98FAB13F9805';  -- Umraniye

-- 1. MEVCUT TEST ILANINI DUZELT
UPDATE Products SET
  Title = N'Akkaraman Koyun - Disi - Saglikli - Istanbul',
  Slug = 'akkaraman-koyun-disi-saglikli-istanbul',
  ShortDescription = N'Akkaraman cinsi, saglikli disi koyun. Damizlik olarak uygundur. Istanbul''da teslim.',
  Description = N'## Hayvan Bilgileri
- **Cins/Irk:** Akkaraman
- **Kategori:** Koyun
- **Kondisyon:** Sifir/Yeni

## Konum
Istanbul, Turkiye

## Fiyat
**12.000 TRY**

---
*Yilmaz Hayvancilik tarafindan yayinlanmistir.*',
  MetaTitle = N'Akkaraman Koyun - Disi - Istanbul',
  MetaDescription = N'Akkaraman cinsi, saglikli disi koyun. Damizlik olarak uygundur.',
  Status = 2,
  PublishedAt = @now,
  ExpiresAt = @expires,
  IsInStock = 1,
  BasePrice = 12000,
  Currency = 'TRY',
  UpdatedAt = @now
WHERE Id = 'FB00EBFC-73B6-480E-AE90-8AB81A896315';

PRINT 'Mevcut ilan guncellendi.';

-- 2. YENI DEMO ILANLAR
INSERT INTO Products (Id, Title, Slug, ShortDescription, Description, CategoryId, BrandId, BasePrice, Currency, DiscountedPrice, PriceUnit, StockQuantity, StockUnit, MinOrderQuantity, MaxOrderQuantity, IsInStock, SellerId, LocationId, Status, Condition, IsShippingAvailable, ShippingCost, IsInternationalShipping, Weight, WeightUnit, Attributes, MetaTitle, MetaDescription, MetaKeywords, ViewCount, FavoriteCount, AverageRating, ReviewCount, PublishedAt, ExpiresAt, MediaBucketId, CoverImageFileId, IsFeatured, FeaturedUntil, BoostScore, IsDeleted, CreatedAt, UpdatedAt)
VALUES
(NEWID(), N'Holstein Sut Inegi - 3 Yasinda - Gunluk 28 Litre - Ankara', 'holstein-sut-inegi-3-yasinda-ankara',
 N'Holstein cinsi, 3 yasinda, gunluk 28 litre sut verimi olan saglikli sut inegi.',
 N'## Hayvan Bilgileri
- **Cins/Irk:** Holstein
- **Cinsiyet:** Disi
- **Yas:** 36 ay (3 yasinda)
- **Agirlik:** 550 kg
- **Amac:** Sut Uretimi

## Saglik Durumu
- **Genel Durum:** Saglikli
- **Gunluk Sut Uretimi:** 28 litre
- **Asilar:** Guncel
- **Dogum Sayisi:** 2

## Konum
Ankara, Turkiye

## Fiyat
**185.000 TRY**

---
*Yilmaz Hayvancilik tarafindan yayinlanmistir.*',
 'E1000000-0000-0000-0000-000000000101', NULL, 185000, 'TRY', NULL, N'Bas', 1, N'Bas', NULL, NULL, 1, @s1, @locAnk, 2, 0, 1, 2500, 0, 550, 'kg', NULL, N'Holstein Sut Inegi - Ankara', N'Holstein cinsi, gunluk 28 litre sut verimi, saglikli sut inegi.', N'holstein,sut inegi,ankara', 45, 8, NULL, 0, @now, @expires, NULL, NULL, 1, DATEADD(DAY,30,@now), 2, 0, @now, @now),

(NEWID(), N'Simental Besi Danasi - Erkek - 8 Aylik - Bolu', 'simental-besi-danasi-erkek-8-aylik-bolu',
 N'Simental cinsi, 8 aylik, 280 kg erkek besi danasi. Bolu''da teslim.',
 N'## Hayvan Bilgileri
- **Cins/Irk:** Simental
- **Cinsiyet:** Erkek
- **Yas:** 8 ay
- **Agirlik:** 280 kg
- **Amac:** Besicilik

## Konum
Bolu, Turkiye

## Fiyat
**95.000 TRY**

---
*Ahmet Test tarafindan yayinlanmistir.*',
 'E1000000-0000-0000-0000-000000000102', NULL, 95000, 'TRY', NULL, N'Bas', 3, N'Bas', 1, NULL, 1, @s2, @locBol, 2, 0, 1, 3000, 0, 280, 'kg', NULL, N'Simental Besi Danasi - Bolu', N'Simental cinsi, 8 aylik, 280 kg erkek besi danasi.', N'simental,besi danasi,bolu', 32, 5, NULL, 0, @now, @expires, NULL, NULL, 0, NULL, 0, 0, @now, @now),

(NEWID(), N'Angus Damizlik Boga - 4 Yasinda - Istanbul', 'angus-damizlik-boga-4-yasinda-istanbul',
 N'Angus cinsi, 4 yasinda, 820 kg damizlik boga. Soy kutugu belgeli.',
 N'## Hayvan Bilgileri
- **Cins/Irk:** Angus
- **Cinsiyet:** Erkek
- **Yas:** 48 ay (4 yasinda)
- **Agirlik:** 820 kg
- **Amac:** Damizlik

## Konum
Istanbul, Turkiye

## Fiyat
**320.000 TRY**

---
*Mustafa Ocak tarafindan yayinlanmistir.*',
 'E1000000-0000-0000-0000-000000000103', NULL, 320000, 'TRY', NULL, N'Bas', 1, N'Bas', NULL, NULL, 1, @s3, @locIst, 2, 0, 0, NULL, 0, 820, 'kg', NULL, N'Angus Damizlik Boga - Istanbul', N'Angus cinsi, 4 yasinda, 820 kg damizlik boga.', N'angus,damizlik boga,istanbul', 67, 12, NULL, 0, @now, @expires, NULL, NULL, 1, DATEADD(DAY,14,@now), 3, 0, @now, @now),

(NEWID(), N'Saanen Sut Kecisi - Disi - 2 Yasinda - Pendik', 'saanen-sut-kecisi-disi-2-yasinda-pendik',
 N'Saanen cinsi, 2 yasinda, gunluk 4 litre sut veren disi keci.',
 N'## Hayvan Bilgileri
- **Cins/Irk:** Saanen
- **Cinsiyet:** Disi
- **Yas:** 24 ay
- **Agirlik:** 65 kg
- **Amac:** Sut Uretimi

## Konum
Pendik, Istanbul, Turkiye

## Fiyat
**18.000 TRY**

---
*Yilmaz Hayvancilik tarafindan yayinlanmistir.*',
 'E1000000-0000-0000-0000-000000000202', NULL, 18000, 'TRY', NULL, N'Bas', 5, N'Bas', 1, NULL, 1, @s1, @locPen, 2, 0, 1, 1500, 0, 65, 'kg', NULL, N'Saanen Sut Kecisi - Pendik', N'Saanen cinsi, gunluk 4 litre sut veren disi keci.', N'saanen,sut kecisi,pendik', 28, 4, NULL, 0, @now, @expires, NULL, NULL, 0, NULL, 0, 0, @now, @now),

(NEWID(), N'Akkaraman Kuzu - 4 Aylik - 5 Baslik Parti - Ankara', 'akkaraman-kuzu-4-aylik-5-baslik-ankara',
 N'Akkaraman cinsi, 4 aylik, 5 baslik kuzu partisi. Besicilige uygun.',
 N'## Hayvan Bilgileri
- **Cins/Irk:** Akkaraman
- **Yas:** 4 ay
- **Agirlik:** 25-30 kg
- **Amac:** Besicilik

## Konum
Ankara, Turkiye

## Fiyat
**8.500 TRY / Bas** (5 baslik parti)

---
*Ahmet Test tarafindan yayinlanmistir.*',
 'E1000000-0000-0000-0000-000000000203', NULL, 8500, 'TRY', NULL, N'Bas', 5, N'Bas', 5, 5, 1, @s2, @locAnk, 2, 0, 1, 2000, 0, 28, 'kg', NULL, N'Akkaraman Kuzu - Ankara', N'Akkaraman cinsi, 4 aylik kuzular, 5 baslik parti.', N'akkaraman,kuzu,ankara', 19, 3, NULL, 0, @now, @expires, NULL, NULL, 0, NULL, 0, 0, @now, @now),

(NEWID(), N'Arap Ati - Erkek - 5 Yasinda - Istanbul', 'arap-ati-erkek-5-yasinda-istanbul',
 N'Safkan Arap ati, 5 yasinda, 450 kg, egitimli erkek at.',
 N'## Hayvan Bilgileri
- **Cins/Irk:** Arap Ati (Safkan)
- **Cinsiyet:** Erkek
- **Yas:** 60 ay (5 yasinda)
- **Agirlik:** 450 kg
- **Amac:** Gosteri/Yaris

## Konum
Istanbul, Turkiye

## Fiyat
**750.000 TRY**

---
*Mustafa Ocak tarafindan yayinlanmistir.*',
 'E1000000-0000-0000-0000-000000000401', NULL, 750000, 'TRY', NULL, N'Bas', 1, N'Bas', NULL, NULL, 1, @s3, @locIst, 2, 0, 0, NULL, 0, 450, 'kg', NULL, N'Arap Ati - Istanbul', N'Safkan Arap ati, 5 yasinda, 450 kg.', N'arap ati,safkan,istanbul', 89, 15, NULL, 0, @now, @expires, NULL, NULL, 1, DATEADD(DAY,7,@now), 5, 0, @now, @now),

(NEWID(), N'Lohmann Brown Yumurta Tavugu - 50 Adetlik Parti - Umraniye', 'lohmann-brown-yumurta-tavugu-50-adet-umraniye',
 N'Lohmann Brown cinsi yumurta tavuklari. 6 aylik, yumurtlamaya baslamis.',
 N'## Hayvan Bilgileri
- **Cins/Irk:** Lohmann Brown
- **Yas:** 6 ay
- **Amac:** Yumurta Uretimi

## Konum
Umraniye, Istanbul, Turkiye

## Fiyat
**350 TRY / Adet** (50 adetlik parti)

---
*Yilmaz Hayvancilik tarafindan yayinlanmistir.*',
 'E1000000-0000-0000-0000-000000000301', NULL, 350, 'TRY', NULL, N'Adet', 50, N'Adet', 10, NULL, 1, @s1, @locUmr, 2, 0, 1, 500, 0, 2, 'kg', NULL, N'Lohmann Brown Yumurta Tavugu - Umraniye', N'Lohmann Brown cinsi yumurta tavuklari, 50 adet.', N'lohmann brown,tavuk,umraniye', 34, 6, NULL, 0, @now, @expires, NULL, NULL, 0, NULL, 0, 0, @now, @now),

(NEWID(), N'Bronz Hindi - Erkek - 8 Aylik - Bolu', 'bronz-hindi-erkek-8-aylik-bolu',
 N'Bronz cinsi, 8 aylik, 12 kg erkek hindi.',
 N'## Hayvan Bilgileri
- **Cins/Irk:** Bronz Hindi
- **Cinsiyet:** Erkek
- **Yas:** 8 ay
- **Agirlik:** 12 kg

## Konum
Bolu, Turkiye

## Fiyat
**2.500 TRY**

---
*Ahmet Test tarafindan yayinlanmistir.*',
 'E1000000-0000-0000-0000-000000000303', NULL, 2500, 'TRY', NULL, N'Bas', 8, N'Bas', 1, NULL, 1, @s2, @locBol, 2, 0, 1, 800, 0, 12, 'kg', NULL, N'Bronz Hindi - Bolu', N'Bronz cinsi, 8 aylik, 12 kg erkek hindi.', N'hindi,bronz,bolu', 15, 2, NULL, 0, @now, @expires, NULL, NULL, 0, NULL, 0, 0, @now, @now),

(NEWID(), N'Kivircik Damizlik Koc - 2 Yasinda - Pendik', 'kivircik-damizlik-koc-2-yasinda-pendik',
 N'Kivircik cinsi, 2 yasinda, 85 kg damizlik koc.',
 N'## Hayvan Bilgileri
- **Cins/Irk:** Kivircik
- **Cinsiyet:** Erkek
- **Yas:** 24 ay
- **Agirlik:** 85 kg
- **Amac:** Damizlik

## Konum
Pendik, Istanbul, Turkiye

## Fiyat
**35.000 TRY**

---
*Mustafa Ocak tarafindan yayinlanmistir.*',
 'E1000000-0000-0000-0000-000000000205', NULL, 35000, 'TRY', NULL, N'Bas', 1, N'Bas', NULL, NULL, 1, @s3, @locPen, 2, 0, 1, 1500, 0, 85, 'kg', NULL, N'Kivircik Damizlik Koc - Pendik', N'Kivircik cinsi, 2 yasinda, 85 kg damizlik koc.', N'kivircik,koc,damizlik,pendik', 41, 7, NULL, 0, @now, @expires, NULL, NULL, 0, NULL, 0, 0, @now, @now),

(NEWID(), N'Montofon Duve - 18 Aylik - Gebe - Ankara', 'montofon-duve-18-aylik-gebe-ankara',
 N'Montofon (Brown Swiss) cinsi, 18 aylik, 420 kg gebe duve.',
 N'## Hayvan Bilgileri
- **Cins/Irk:** Montofon (Brown Swiss)
- **Cinsiyet:** Disi
- **Yas:** 18 ay
- **Agirlik:** 420 kg
- **Amac:** Sut Uretimi

## Saglik Durumu
- **Gebelik:** Evet (tahmini dogum: 3 ay sonra)

## Konum
Ankara, Turkiye

## Fiyat
**145.000 TRY**

---
*Yilmaz Hayvancilik tarafindan yayinlanmistir.*',
 'E1000000-0000-0000-0000-000000000105', NULL, 145000, 'TRY', NULL, N'Bas', 1, N'Bas', NULL, NULL, 1, @s1, @locAnk, 2, 0, 1, 2500, 0, 420, 'kg', NULL, N'Montofon Duve - Gebe - Ankara', N'Montofon cinsi, 18 aylik, 420 kg gebe duve.', N'montofon,duve,gebe,ankara', 53, 9, NULL, 0, @now, @expires, NULL, NULL, 0, NULL, 0, 0, @now, @now),

(NEWID(), N'Japon Bildircini - 100 Adetlik Parti - Umraniye', 'japon-bildircini-100-adet-umraniye',
 N'Japon bildircini, yumurta verimli, 100 adetlik parti.',
 N'## Hayvan Bilgileri
- **Cins/Irk:** Japon Bildircini
- **Amac:** Yumurta Uretimi

## Konum
Umraniye, Istanbul, Turkiye

## Fiyat
**45 TRY / Adet** (100 adetlik parti)

---
*Ahmet Test tarafindan yayinlanmistir.*',
 'E1000000-0000-0000-0000-000000000306', NULL, 45, 'TRY', NULL, N'Adet', 100, N'Adet', 20, NULL, 1, @s2, @locUmr, 2, 0, 1, 300, 0, 0.2, 'kg', NULL, N'Japon Bildircini - Umraniye', N'Japon bildircini, yumurta verimli, 100 adet.', N'bildircin,japon,umraniye', 22, 3, NULL, 0, @now, @expires, NULL, NULL, 0, NULL, 0, 0, @now, @now),

(NEWID(), N'Saanen Oglak - 3 Aylik - 10 Baslik - Bolu', 'saanen-oglak-3-aylik-10-baslik-bolu',
 N'Saanen cinsi, 3 aylik, 10 baslik oglak partisi.',
 N'## Hayvan Bilgileri
- **Cins/Irk:** Saanen
- **Yas:** 3 ay
- **Agirlik:** 15-18 kg

## Konum
Bolu, Turkiye

## Fiyat
**5.500 TRY / Bas** (10 baslik parti)

---
*Mustafa Ocak tarafindan yayinlanmistir.*',
 'E1000000-0000-0000-0000-000000000204', NULL, 5500, 'TRY', NULL, N'Bas', 10, N'Bas', 5, NULL, 1, @s3, @locBol, 2, 0, 1, 1200, 0, 17, 'kg', NULL, N'Saanen Oglak - Bolu', N'Saanen cinsi, 3 aylik oglaklar, 10 baslik parti.', N'saanen,oglak,bolu', 18, 2, NULL, 0, @now, @expires, NULL, NULL, 0, NULL, 0, 0, @now, @now),

(NEWID(), N'Toulouse Kazi - Cift - Istanbul', 'toulouse-kazi-cift-istanbul',
 N'Toulouse cinsi kaz cifti. Damizlik olarak uygundur.',
 N'## Hayvan Bilgileri
- **Cins/Irk:** Toulouse
- **Yas:** 12 ay

## Konum
Istanbul, Turkiye

## Fiyat
**4.000 TRY / Cift**

---
*Yilmaz Hayvancilik tarafindan yayinlanmistir.*',
 'E1000000-0000-0000-0000-000000000304', NULL, 4000, 'TRY', NULL, N'Cift', 3, N'Cift', 1, NULL, 1, @s1, @locIst, 2, 0, 1, 800, 0, 8, 'kg', NULL, N'Toulouse Kazi - Istanbul', N'Toulouse cinsi kaz cifti, damizlik.', N'kaz,toulouse,istanbul', 14, 1, NULL, 0, @now, @expires, NULL, NULL, 0, NULL, 0, 0, @now, @now),

(NEWID(), N'Anadolu Esegi - Disi - 6 Yasinda - Ankara', 'anadolu-esegi-disi-6-yasinda-ankara',
 N'Anadolu esegi, 6 yasinda, uysal, yuk tasima ve ciftlik islerinde deneyimli.',
 N'## Hayvan Bilgileri
- **Cins/Irk:** Anadolu Esegi
- **Cinsiyet:** Disi
- **Yas:** 72 ay (6 yasinda)
- **Agirlik:** 220 kg

## Konum
Ankara, Turkiye

## Fiyat
**25.000 TRY**

---
*Ahmet Test tarafindan yayinlanmistir.*',
 'E1000000-0000-0000-0000-000000000402', NULL, 25000, 'TRY', NULL, N'Bas', 1, N'Bas', NULL, NULL, 1, @s2, @locAnk, 2, 0, 0, NULL, 0, 220, 'kg', NULL, N'Anadolu Esegi - Ankara', N'Anadolu esegi, 6 yasinda, uysal, is hayvani.', N'esek,anadolu,ankara', 11, 1, NULL, 0, @now, @expires, NULL, NULL, 0, NULL, 0, 0, @now, @now),

(NEWID(), N'Anadolu Mandasi - Disi - 4 Yasinda - Istanbul', 'anadolu-mandasi-disi-4-yasinda-istanbul',
 N'Anadolu mandasi, 4 yasinda, gunluk 8 litre sut veren disi manda.',
 N'## Hayvan Bilgileri
- **Cins/Irk:** Anadolu Mandasi
- **Cinsiyet:** Disi
- **Yas:** 48 ay (4 yasinda)
- **Agirlik:** 480 kg

## Konum
Istanbul, Turkiye

## Fiyat
**120.000 TRY**

---
*Mustafa Ocak tarafindan yayinlanmistir.*',
 'E1000000-0000-0000-0000-000000000106', NULL, 120000, 'TRY', NULL, N'Bas', 1, N'Bas', NULL, NULL, 1, @s3, @locIst, 2, 0, 0, NULL, 0, 480, 'kg', NULL, N'Anadolu Mandasi - Istanbul', N'Anadolu mandasi, gunluk 8 litre sut, disi manda.', N'manda,anadolu,istanbul', 37, 6, NULL, 0, @now, @expires, NULL, NULL, 0, NULL, 0, 0, @now, @now),

(NEWID(), N'Holstein Buzagi - Disi - 2 Haftalik - Pendik', 'holstein-buzagi-disi-2-haftalik-pendik',
 N'Holstein cinsi, 2 haftalik disi buzagi.',
 N'## Hayvan Bilgileri
- **Cins/Irk:** Holstein
- **Cinsiyet:** Disi
- **Yas:** 2 haftalik
- **Agirlik:** 45 kg

## Konum
Pendik, Istanbul, Turkiye

## Fiyat
**28.000 TRY**

---
*Yilmaz Hayvancilik tarafindan yayinlanmistir.*',
 'E1000000-0000-0000-0000-000000000104', NULL, 28000, 'TRY', NULL, N'Bas', 2, N'Bas', 1, NULL, 1, @s1, @locPen, 2, 0, 1, 1500, 0, 45, 'kg', NULL, N'Holstein Buzagi - Pendik', N'Holstein cinsi, 2 haftalik disi buzagi.', N'buzagi,holstein,pendik', 26, 4, NULL, 0, @now, @expires, NULL, NULL, 0, NULL, 0, 0, @now, @now),

(NEWID(), N'Afrika Deve Kusu - Cift - 2 Yasinda - Bolu', 'afrika-deve-kusu-cift-2-yasinda-bolu',
 N'Afrika deve kusu cifti, 2 yasinda, damizlik olarak uygundur.',
 N'## Hayvan Bilgileri
- **Cins/Irk:** Afrika Deve Kusu
- **Yas:** 24 ay

## Konum
Bolu, Turkiye

## Fiyat
**65.000 TRY / Cift**

---
*Ahmet Test tarafindan yayinlanmistir.*',
 'E1000000-0000-0000-0000-000000000307', NULL, 65000, 'TRY', NULL, N'Cift', 1, N'Cift', NULL, NULL, 1, @s2, @locBol, 2, 0, 0, NULL, 0, 120, 'kg', NULL, N'Afrika Deve Kusu - Bolu', N'Afrika deve kusu cifti, 2 yasinda, damizlik.', N'deve kusu,afrika,bolu', 43, 8, NULL, 0, @now, @expires, NULL, NULL, 1, DATEADD(DAY,21,@now), 4, 0, @now, @now),

(NEWID(), N'Ross 308 Broiler Civciv - 500 Adetlik Parti - Umraniye', 'ross-308-broiler-civciv-500-adet-umraniye',
 N'Ross 308 cinsi broiler civciv, 1 gunluk, 500 adetlik parti.',
 N'## Hayvan Bilgileri
- **Cins/Irk:** Ross 308
- **Yas:** 1 gunluk

## Konum
Umraniye, Istanbul, Turkiye

## Fiyat
**25 TRY / Adet** (500 adetlik parti)

---
*Mustafa Ocak tarafindan yayinlanmistir.*',
 'E1000000-0000-0000-0000-000000000302', NULL, 25, 'TRY', NULL, N'Adet', 500, N'Adet', 500, NULL, 1, @s3, @locUmr, 2, 0, 1, 1000, 0, 0.04, 'kg', NULL, N'Ross 308 Broiler Civciv - Umraniye', N'Ross 308 broiler civciv, 1 gunluk, 500 adet.', N'civciv,ross 308,broiler,umraniye', 58, 10, NULL, 0, @now, @expires, NULL, NULL, 0, NULL, 0, 0, @now, @now),

(NEWID(), N'Boer Teke - 18 Aylik - Damizlik - Ankara', 'boer-teke-18-aylik-damizlik-ankara',
 N'Boer cinsi, 18 aylik, 75 kg damizlik teke.',
 N'## Hayvan Bilgileri
- **Cins/Irk:** Boer
- **Cinsiyet:** Erkek
- **Yas:** 18 ay
- **Agirlik:** 75 kg
- **Amac:** Damizlik

## Konum
Ankara, Turkiye

## Fiyat
**22.000 TRY**

---
*Yilmaz Hayvancilik tarafindan yayinlanmistir.*',
 'E1000000-0000-0000-0000-000000000206', NULL, 22000, 'TRY', NULL, N'Bas', 1, N'Bas', NULL, NULL, 1, @s1, @locAnk, 2, 0, 1, 1200, 0, 75, 'kg', NULL, N'Boer Teke - Damizlik - Ankara', N'Boer cinsi, 18 aylik, 75 kg damizlik teke.', N'boer,teke,damizlik,ankara', 16, 2, NULL, 0, @now, @expires, NULL, NULL, 0, NULL, 0, 0, @now, @now),

(NEWID(), N'Angora Tavsani - Cift - 6 Aylik - Istanbul', 'angora-tavsani-cift-6-aylik-istanbul',
 N'Angora tavsani cifti, 6 aylik, evcil hayvan olarak uygundur.',
 N'## Hayvan Bilgileri
- **Cins/Irk:** Angora Tavsani
- **Yas:** 6 ay

## Konum
Istanbul, Turkiye

## Fiyat
**3.500 TRY / Cift**

---
*Ahmet Test tarafindan yayinlanmistir.*',
 'E1000000-0000-0000-0000-000000001101', NULL, 3500, 'TRY', NULL, N'Cift', 4, N'Cift', 1, NULL, 1, @s2, @locIst, 2, 0, 1, 500, 0, 3, 'kg', NULL, N'Angora Tavsani - Istanbul', N'Angora tavsani cifti, 6 aylik.', N'angora,tavsan,istanbul', 31, 5, NULL, 0, @now, @expires, NULL, NULL, 0, NULL, 0, 0, @now, @now);

-- Sonuc
SELECT COUNT(*) AS ToplamAktifIlan FROM Products WHERE IsDeleted = 0 AND Status = 2;

COMMIT TRANSACTION;
GO
