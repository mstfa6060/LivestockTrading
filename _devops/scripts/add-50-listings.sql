BEGIN TRANSACTION;

DECLARE @now DATETIME2 = GETUTCDATE();
DECLARE @exp DATETIME2 = DATEADD(DAY, 60, GETUTCDATE());

DECLARE @s1 UNIQUEIDENTIFIER = 'E4ADB651-9AF2-4DD7-86F8-DED18E3D5577'; -- Yilmaz Hayvancilik
DECLARE @s2 UNIQUEIDENTIFIER = '57EF0F5B-86EE-43E8-8546-402B671D39EE'; -- Ahmet Test
DECLARE @s3 UNIQUEIDENTIFIER = '180A26F4-0B6F-48DF-BA79-691D0E221F42'; -- Mustafa Ocak

DECLARE @ist UNIQUEIDENTIFIER = 'C0E1253E-A1E4-4868-A7F6-BB33169EBF72';
DECLARE @ank UNIQUEIDENTIFIER = '930F3267-BD23-4508-B28E-0F9601E3D14B';
DECLARE @bol UNIQUEIDENTIFIER = 'FBD10477-FDA1-4D0E-A379-7D453BE7F0E4';
DECLARE @pen UNIQUEIDENTIFIER = 'AD53775B-9794-492D-9EF9-CFF9DF333C8D';
DECLARE @umr UNIQUEIDENTIFIER = '4C8B4DB3-3504-4417-A570-98FAB13F9805';

INSERT INTO Products (Id, Title, Slug, ShortDescription, Description, CategoryId, BrandId, BasePrice, Currency, DiscountedPrice, PriceUnit, StockQuantity, StockUnit, MinOrderQuantity, MaxOrderQuantity, IsInStock, SellerId, LocationId, Status, Condition, IsShippingAvailable, ShippingCost, IsInternationalShipping, Weight, WeightUnit, Attributes, MetaTitle, MetaDescription, MetaKeywords, ViewCount, FavoriteCount, AverageRating, ReviewCount, PublishedAt, ExpiresAt, MediaBucketId, CoverImageFileId, IsFeatured, FeaturedUntil, BoostScore, IsDeleted, CreatedAt, UpdatedAt)
VALUES
-- ═══ BUYUKBAS - SIGER IRKLARI ═══
-- 1) Jersey Sut Inegi
(NEWID(), N'Jersey Sut Inegi - 4 Yasinda - Gunluk 22 Litre - Bolu', 'jersey-sut-inegi-4-yasinda-bolu',
 N'Jersey cinsi, 4 yasinda, gunluk 22 litre sut verimi olan saglikli sut inegi.',
 N'## Hayvan Bilgileri
- **Cins/Irk:** Jersey
- **Cinsiyet:** Disi
- **Yas:** 48 ay
- **Agirlik:** 420 kg
- **Amac:** Sut Uretimi
- **Gunluk Sut:** 22 litre (yuksek yag oranli)

## Konum
Bolu, Turkiye

## Fiyat
**165.000 TRY**',
 'E1000000-0000-0000-0000-000000010102', NULL, 165000, 'TRY', NULL, N'Bas', 1, N'Bas', NULL, NULL, 1, @s1, @bol, 2, 0, 1, 2500, 0, 420, 'kg', NULL, N'Jersey Sut Inegi - Bolu', N'Jersey cinsi, gunluk 22 litre sut verimi.', N'jersey,sut inegi,bolu', 38, 6, NULL, 0, @now, @exp, NULL, NULL, 0, NULL, 0, 0, @now, @now),

-- 2) Simmental Duve
(NEWID(), N'Simmental Duve - 14 Aylik - Ankara', 'simmental-duve-14-aylik-ankara',
 N'Simmental cinsi, 14 aylik, 380 kg duve. Sut ve et icin cift amacli.',
 N'## Hayvan Bilgileri
- **Cins/Irk:** Simmental
- **Cinsiyet:** Disi
- **Yas:** 14 ay
- **Agirlik:** 380 kg
- **Amac:** Cift amacli (Sut + Et)

## Konum
Ankara, Turkiye

## Fiyat
**130.000 TRY**',
 'E1000000-0000-0000-0000-000000010104', NULL, 130000, 'TRY', NULL, N'Bas', 2, N'Bas', NULL, NULL, 1, @s2, @ank, 2, 0, 1, 2500, 0, 380, 'kg', NULL, N'Simmental Duve - Ankara', N'Simmental cinsi, 14 aylik duve.', N'simmental,duve,ankara', 29, 4, NULL, 0, @now, @exp, NULL, NULL, 0, NULL, 0, 0, @now, @now),

-- 3) Hereford Besi Sigiri
(NEWID(), N'Hereford Besi Sigiri - Erkek - 12 Aylik - Istanbul', 'hereford-besi-sigiri-erkek-istanbul',
 N'Hereford cinsi, 12 aylik, 380 kg erkek besi sigiri.',
 N'## Hayvan Bilgileri
- **Cins/Irk:** Hereford
- **Cinsiyet:** Erkek
- **Yas:** 12 ay
- **Agirlik:** 380 kg
- **Amac:** Besicilik

## Konum
Istanbul, Turkiye

## Fiyat
**110.000 TRY**',
 'E1000000-0000-0000-0000-000000010202', NULL, 110000, 'TRY', NULL, N'Bas', 2, N'Bas', NULL, NULL, 1, @s3, @ist, 2, 0, 1, 3000, 0, 380, 'kg', NULL, N'Hereford Besi Sigiri - Istanbul', N'Hereford cinsi, 12 aylik erkek besi sigiri.', N'hereford,besi,istanbul', 44, 7, NULL, 0, @now, @exp, NULL, NULL, 0, NULL, 0, 0, @now, @now),

-- 4) Charolais Buzagi
(NEWID(), N'Charolais Buzagi - Erkek - 3 Aylik - Bolu', 'charolais-buzagi-erkek-3-aylik-bolu',
 N'Charolais cinsi, 3 aylik, 120 kg erkek buzagi.',
 N'## Hayvan Bilgileri
- **Cins/Irk:** Charolais
- **Cinsiyet:** Erkek
- **Yas:** 3 ay
- **Agirlik:** 120 kg

## Konum
Bolu, Turkiye

## Fiyat
**55.000 TRY**',
 'E1000000-0000-0000-0000-000000010203', NULL, 55000, 'TRY', NULL, N'Bas', 3, N'Bas', 1, NULL, 1, @s1, @bol, 2, 0, 1, 2000, 0, 120, 'kg', NULL, N'Charolais Buzagi - Bolu', N'Charolais cinsi, 3 aylik erkek buzagi.', N'charolais,buzagi,bolu', 21, 3, NULL, 0, @now, @exp, NULL, NULL, 0, NULL, 0, 0, @now, @now),

-- 5) Limousin Damizlik Boga
(NEWID(), N'Limousin Damizlik Boga - 3 Yasinda - Ankara', 'limousin-damizlik-boga-3-yasinda-ankara',
 N'Limousin cinsi, 3 yasinda, 900 kg damizlik boga. Soy kutugu belgeli.',
 N'## Hayvan Bilgileri
- **Cins/Irk:** Limousin
- **Cinsiyet:** Erkek
- **Yas:** 36 ay
- **Agirlik:** 900 kg
- **Amac:** Damizlik

## Konum
Ankara, Turkiye

## Fiyat
**280.000 TRY**',
 'E1000000-0000-0000-0000-000000010204', NULL, 280000, 'TRY', NULL, N'Bas', 1, N'Bas', NULL, NULL, 1, @s2, @ank, 2, 0, 0, NULL, 0, 900, 'kg', NULL, N'Limousin Damizlik Boga - Ankara', N'Limousin cinsi, 3 yasinda, 900 kg damizlik boga.', N'limousin,boga,damizlik,ankara', 56, 11, NULL, 0, @now, @exp, NULL, NULL, 1, DATEADD(DAY,20,@now), 3, 0, @now, @now),

-- 6) Murrah Mandasi
(NEWID(), N'Murrah Mandasi - Disi - 3 Yasinda - Pendik', 'murrah-mandasi-disi-3-yasinda-pendik',
 N'Murrah cinsi, 3 yasinda, gunluk 12 litre sut veren disi manda.',
 N'## Hayvan Bilgileri
- **Cins/Irk:** Murrah
- **Cinsiyet:** Disi
- **Yas:** 36 ay
- **Agirlik:** 520 kg
- **Gunluk Sut:** 12 litre

## Konum
Pendik, Istanbul, Turkiye

## Fiyat
**160.000 TRY**',
 'E1000000-0000-0000-0000-000000010602', NULL, 160000, 'TRY', NULL, N'Bas', 1, N'Bas', NULL, NULL, 1, @s3, @pen, 2, 0, 0, NULL, 0, 520, 'kg', NULL, N'Murrah Mandasi - Pendik', N'Murrah cinsi, gunluk 12 litre sut, disi manda.', N'murrah,manda,pendik', 33, 5, NULL, 0, @now, @exp, NULL, NULL, 0, NULL, 0, 0, @now, @now),

-- ═══ KUCUKBAS - KOYUN IRKLARI ═══
-- 7) Merinos Koyun
(NEWID(), N'Merinos Koyun - 10 Baslik Suru - Ankara', 'merinos-koyun-10-baslik-suru-ankara',
 N'Merinos cinsi, 2-3 yasinda, 10 baslik koyun surusu. Yun ve et icin ideal.',
 N'## Hayvan Bilgileri
- **Cins/Irk:** Merinos
- **Yas:** 2-3 yas arasi
- **Agirlik:** 60-75 kg

## Konum
Ankara, Turkiye

## Fiyat
**15.000 TRY / Bas** (10 baslik suru)',
 'E1000000-0000-0000-0000-000000020101', NULL, 15000, 'TRY', NULL, N'Bas', 10, N'Bas', 5, NULL, 1, @s1, @ank, 2, 0, 1, 2000, 0, 65, 'kg', NULL, N'Merinos Koyun - 10 Baslik - Ankara', N'Merinos cinsi, 10 baslik koyun surusu.', N'merinos,koyun,ankara', 47, 8, NULL, 0, @now, @exp, NULL, NULL, 0, NULL, 0, 0, @now, @now),

-- 8) Dorper Koyun
(NEWID(), N'Dorper Koyun - Disi - 2 Yasinda - Bolu', 'dorper-koyun-disi-2-yasinda-bolu',
 N'Dorper cinsi, 2 yasinda, 70 kg disi koyun. Et irki, hizli gelisim.',
 N'## Hayvan Bilgileri
- **Cins/Irk:** Dorper
- **Cinsiyet:** Disi
- **Yas:** 24 ay
- **Agirlik:** 70 kg
- **Amac:** Besicilik

## Konum
Bolu, Turkiye

## Fiyat
**18.000 TRY**',
 'E1000000-0000-0000-0000-000000020103', NULL, 18000, 'TRY', NULL, N'Bas', 3, N'Bas', 1, NULL, 1, @s2, @bol, 2, 0, 1, 1500, 0, 70, 'kg', NULL, N'Dorper Koyun - Bolu', N'Dorper cinsi, 2 yasinda, 70 kg disi koyun.', N'dorper,koyun,bolu', 23, 3, NULL, 0, @now, @exp, NULL, NULL, 0, NULL, 0, 0, @now, @now),

-- 9) Ivesi Sut Koyunu
(NEWID(), N'Ivesi Sut Koyunu - Disi - 3 Yasinda - Istanbul', 'ivesi-sut-koyunu-disi-istanbul',
 N'Ivesi cinsi, 3 yasinda sut koyunu. Gunluk 2 litre sut verimi.',
 N'## Hayvan Bilgileri
- **Cins/Irk:** Ivesi
- **Cinsiyet:** Disi
- **Yas:** 36 ay
- **Amac:** Sut Uretimi

## Konum
Istanbul, Turkiye

## Fiyat
**14.000 TRY**',
 'E1000000-0000-0000-0000-000000020107', NULL, 14000, 'TRY', NULL, N'Bas', 5, N'Bas', 1, NULL, 1, @s3, @ist, 2, 0, 1, 1500, 0, 55, 'kg', NULL, N'Ivesi Sut Koyunu - Istanbul', N'Ivesi cinsi, 3 yasinda sut koyunu.', N'ivesi,sut koyunu,istanbul', 19, 2, NULL, 0, @now, @exp, NULL, NULL, 0, NULL, 0, 0, @now, @now),

-- 10) Suffolk Damizlik Koc
(NEWID(), N'Suffolk Damizlik Koc - 2 Yasinda - Pendik', 'suffolk-damizlik-koc-2-yasinda-pendik',
 N'Suffolk cinsi, 2 yasinda, 95 kg damizlik koc. Et irki.',
 N'## Hayvan Bilgileri
- **Cins/Irk:** Suffolk
- **Cinsiyet:** Erkek
- **Yas:** 24 ay
- **Agirlik:** 95 kg
- **Amac:** Damizlik

## Konum
Pendik, Istanbul, Turkiye

## Fiyat
**40.000 TRY**',
 'E1000000-0000-0000-0000-000000020102', NULL, 40000, 'TRY', NULL, N'Bas', 1, N'Bas', NULL, NULL, 1, @s1, @pen, 2, 0, 1, 1500, 0, 95, 'kg', NULL, N'Suffolk Damizlik Koc - Pendik', N'Suffolk cinsi, 2 yasinda, 95 kg damizlik koc.', N'suffolk,koc,damizlik,pendik', 35, 6, NULL, 0, @now, @exp, NULL, NULL, 0, NULL, 0, 0, @now, @now),

-- ═══ KECI IRKLARI ═══
-- 11) Kilis Kecisi
(NEWID(), N'Kilis Kecisi - Disi - 2 Yasinda - Ankara', 'kilis-kecisi-disi-2-yasinda-ankara',
 N'Kilis cinsi, 2 yasinda, gunluk 3 litre sut veren disi keci.',
 N'## Hayvan Bilgileri
- **Cins/Irk:** Kilis
- **Cinsiyet:** Disi
- **Yas:** 24 ay
- **Amac:** Sut Uretimi

## Konum
Ankara, Turkiye

## Fiyat
**12.000 TRY**',
 'E1000000-0000-0000-0000-000000020205', NULL, 12000, 'TRY', NULL, N'Bas', 4, N'Bas', 1, NULL, 1, @s2, @ank, 2, 0, 1, 1200, 0, 50, 'kg', NULL, N'Kilis Kecisi - Ankara', N'Kilis cinsi, gunluk 3 litre sut veren keci.', N'kilis,keci,ankara', 17, 2, NULL, 0, @now, @exp, NULL, NULL, 0, NULL, 0, 0, @now, @now),

-- 12) Ankara Tiftik Kecisi
(NEWID(), N'Ankara Tiftik Kecisi - 5 Baslik - Ankara', 'ankara-tiftik-kecisi-5-baslik-ankara',
 N'Ankara Tiftik kecisi, 5 baslik parti. Tiftik (mohair) uretimi icin.',
 N'## Hayvan Bilgileri
- **Cins/Irk:** Ankara Tiftik Kecisi
- **Yas:** 1-2 yas
- **Amac:** Tiftik (Mohair) Uretimi

## Konum
Ankara, Turkiye

## Fiyat
**10.000 TRY / Bas** (5 baslik parti)',
 'E1000000-0000-0000-0000-000000020203', NULL, 10000, 'TRY', NULL, N'Bas', 5, N'Bas', 5, 5, 1, @s3, @ank, 2, 0, 1, 1200, 0, 40, 'kg', NULL, N'Ankara Tiftik Kecisi - 5 Baslik', N'Ankara Tiftik kecisi, 5 baslik parti.', N'tiftik,keci,ankara,mohair', 24, 4, NULL, 0, @now, @exp, NULL, NULL, 0, NULL, 0, 0, @now, @now),

-- 13) Sam Kecisi
(NEWID(), N'Sam Kecisi (Damascus) - Disi - 18 Aylik - Istanbul', 'sam-kecisi-disi-18-aylik-istanbul',
 N'Sam (Damascus) cinsi, 18 aylik, sut verimi yuksek disi keci.',
 N'## Hayvan Bilgileri
- **Cins/Irk:** Sam (Damascus)
- **Cinsiyet:** Disi
- **Yas:** 18 ay
- **Amac:** Sut Uretimi

## Konum
Istanbul, Turkiye

## Fiyat
**25.000 TRY**',
 'E1000000-0000-0000-0000-000000020207', NULL, 25000, 'TRY', NULL, N'Bas', 2, N'Bas', NULL, NULL, 1, @s1, @ist, 2, 0, 1, 1500, 0, 60, 'kg', NULL, N'Sam Kecisi - Istanbul', N'Sam (Damascus) cinsi, 18 aylik disi keci.', N'sam kecisi,damascus,istanbul', 42, 7, NULL, 0, @now, @exp, NULL, NULL, 1, DATEADD(DAY,14,@now), 2, 0, @now, @now),

-- ═══ AT IRKLARI ═══
-- 14) Ingiliz Ati
(NEWID(), N'Ingiliz Ati (Thoroughbred) - Erkek - 4 Yasinda - Istanbul', 'ingiliz-ati-erkek-4-yasinda-istanbul',
 N'Safkan Ingiliz ati, 4 yasinda, yaris ve binis icin egitimli.',
 N'## Hayvan Bilgileri
- **Cins/Irk:** Ingiliz Ati (Thoroughbred)
- **Cinsiyet:** Erkek
- **Yas:** 48 ay
- **Agirlik:** 500 kg
- **Amac:** Gosteri/Yaris

## Konum
Istanbul, Turkiye

## Fiyat
**950.000 TRY**',
 'E1000000-0000-0000-0000-000000040102', NULL, 950000, 'TRY', NULL, N'Bas', 1, N'Bas', NULL, NULL, 1, @s2, @ist, 2, 0, 0, NULL, 0, 500, 'kg', NULL, N'Ingiliz Ati - Istanbul', N'Safkan Ingiliz ati, 4 yasinda.', N'ingiliz ati,thoroughbred,istanbul', 112, 22, NULL, 0, @now, @exp, NULL, NULL, 1, DATEADD(DAY,30,@now), 5, 0, @now, @now),

-- 15) Quarter Horse
(NEWID(), N'Quarter Horse - Disi - 6 Yasinda - Bolu', 'quarter-horse-disi-6-yasinda-bolu',
 N'American Quarter Horse, 6 yasinda, uysal, ciftlik isi ve binis icin ideal.',
 N'## Hayvan Bilgileri
- **Cins/Irk:** Quarter Horse
- **Cinsiyet:** Disi
- **Yas:** 72 ay
- **Agirlik:** 480 kg

## Konum
Bolu, Turkiye

## Fiyat
**350.000 TRY**',
 'E1000000-0000-0000-0000-000000040103', NULL, 350000, 'TRY', NULL, N'Bas', 1, N'Bas', NULL, NULL, 1, @s3, @bol, 2, 0, 0, NULL, 0, 480, 'kg', NULL, N'Quarter Horse - Bolu', N'Quarter Horse, 6 yasinda, uysal.', N'quarter horse,at,bolu', 67, 13, NULL, 0, @now, @exp, NULL, NULL, 0, NULL, 0, 0, @now, @now),

-- 16) Katir
(NEWID(), N'Katir - Erkek - 5 Yasinda - Ankara', 'katir-erkek-5-yasinda-ankara',
 N'Guclu katir, 5 yasinda, yuk tasima ve daglik arazide kullanima uygun.',
 N'## Hayvan Bilgileri
- **Cins/Irk:** Katir
- **Cinsiyet:** Erkek
- **Yas:** 60 ay
- **Agirlik:** 350 kg
- **Amac:** Is Hayvani

## Konum
Ankara, Turkiye

## Fiyat
**45.000 TRY**',
 'E1000000-0000-0000-0000-000000000403', NULL, 45000, 'TRY', NULL, N'Bas', 1, N'Bas', NULL, NULL, 1, @s1, @ank, 2, 0, 0, NULL, 0, 350, 'kg', NULL, N'Katir - Ankara', N'Guclu katir, 5 yasinda.', N'katir,ankara,is hayvani', 14, 1, NULL, 0, @now, @exp, NULL, NULL, 0, NULL, 0, 0, @now, @now),

-- ═══ DEVE ═══
-- 17) Tek Horguclu Deve
(NEWID(), N'Tek Horguclu Deve - Erkek - 7 Yasinda - Ankara', 'tek-horguclu-deve-erkek-ankara',
 N'Tek horguclu deve (dromedary), 7 yasinda, 600 kg. Yuk tasima ve sut icin.',
 N'## Hayvan Bilgileri
- **Cins/Irk:** Tek Horguclu Deve (Dromedary)
- **Cinsiyet:** Erkek
- **Yas:** 84 ay
- **Agirlik:** 600 kg

## Konum
Ankara, Turkiye

## Fiyat
**180.000 TRY**',
 'E1000000-0000-0000-0000-000000110301', NULL, 180000, 'TRY', NULL, N'Bas', 1, N'Bas', NULL, NULL, 1, @s2, @ank, 2, 0, 0, NULL, 0, 600, 'kg', NULL, N'Tek Horguclu Deve - Ankara', N'Tek horguclu deve, 7 yasinda, 600 kg.', N'deve,dromedary,ankara', 51, 9, NULL, 0, @now, @exp, NULL, NULL, 1, DATEADD(DAY,14,@now), 3, 0, @now, @now),

-- 18) Yaris Devesi
(NEWID(), N'Yaris Devesi - Erkek - 4 Yasinda - Istanbul', 'yaris-devesi-erkek-istanbul',
 N'Yaris devesi, 4 yasinda, antrenmanli, hizli ve dayanikli.',
 N'## Hayvan Bilgileri
- **Cins/Irk:** Yaris Devesi
- **Cinsiyet:** Erkek
- **Yas:** 48 ay
- **Agirlik:** 450 kg
- **Amac:** Gosteri/Yaris

## Konum
Istanbul, Turkiye

## Fiyat
**500.000 TRY**',
 'E1000000-0000-0000-0000-000000110304', NULL, 500000, 'TRY', NULL, N'Bas', 1, N'Bas', NULL, NULL, 1, @s3, @ist, 2, 0, 0, NULL, 0, 450, 'kg', NULL, N'Yaris Devesi - Istanbul', N'Yaris devesi, 4 yasinda.', N'yaris devesi,istanbul', 78, 14, NULL, 0, @now, @exp, NULL, NULL, 1, DATEADD(DAY,21,@now), 4, 0, @now, @now),

-- ═══ CIFTLIK KOPEKLERI ═══
-- 19) Kangal
(NEWID(), N'Kangal Coban Kopegi - Erkek - 18 Aylik - Ankara', 'kangal-coban-kopegi-erkek-ankara',
 N'Soy kutuklu Kangal coban kopegi, 18 aylik erkek. Suru koruma icin egitimli.',
 N'## Hayvan Bilgileri
- **Cins/Irk:** Kangal
- **Cinsiyet:** Erkek
- **Yas:** 18 ay
- **Agirlik:** 55 kg
- **Amac:** Suru Koruma

## Konum
Ankara, Turkiye

## Fiyat
**35.000 TRY**',
 'E1000000-0000-0000-0000-000000110201', NULL, 35000, 'TRY', NULL, N'Bas', 1, N'Bas', NULL, NULL, 1, @s1, @ank, 2, 0, 1, 1000, 0, 55, 'kg', NULL, N'Kangal Coban Kopegi - Ankara', N'Kangal coban kopegi, 18 aylik, erkek.', N'kangal,coban kopegi,ankara', 63, 12, NULL, 0, @now, @exp, NULL, NULL, 0, NULL, 0, 0, @now, @now),

-- 20) Akbas
(NEWID(), N'Akbas Coban Kopegi - Disi - 1 Yasinda - Bolu', 'akbas-coban-kopegi-disi-bolu',
 N'Akbas cinsi coban kopegi, 1 yasinda, disi. Suru ve ciftlik koruma.',
 N'## Hayvan Bilgileri
- **Cins/Irk:** Akbas
- **Cinsiyet:** Disi
- **Yas:** 12 ay
- **Agirlik:** 40 kg

## Konum
Bolu, Turkiye

## Fiyat
**20.000 TRY**',
 'E1000000-0000-0000-0000-000000110202', NULL, 20000, 'TRY', NULL, N'Bas', 2, N'Bas', NULL, NULL, 1, @s2, @bol, 2, 0, 1, 800, 0, 40, 'kg', NULL, N'Akbas Coban Kopegi - Bolu', N'Akbas cinsi coban kopegi, 1 yasinda.', N'akbas,coban kopegi,bolu', 28, 5, NULL, 0, @now, @exp, NULL, NULL, 0, NULL, 0, 0, @now, @now),

-- ═══ BALIKCILIK ═══
-- 21) Alabalik
(NEWID(), N'Gokkusagi Alabaligi - 1000 Adet - Bolu', 'gokkusagi-alabaligi-1000-adet-bolu',
 N'Gokkusagi alabaligi, 250-300 gr, 1000 adetlik canli balik partisi.',
 N'## Hayvan Bilgileri
- **Cins/Irk:** Gokkusagi Alabaligi
- **Agirlik:** 250-300 gr / adet
- **Amac:** Sofralik / Yetistirme

## Konum
Bolu, Turkiye

## Fiyat
**35 TRY / Adet** (1000 adetlik parti)',
 'E1000000-0000-0000-0000-000000100101', NULL, 35, 'TRY', NULL, N'Adet', 1000, N'Adet', 100, NULL, 1, @s3, @bol, 2, 0, 1, 2000, 0, 0.28, 'kg', NULL, N'Gokkusagi Alabaligi - Bolu', N'Gokkusagi alabaligi, 1000 adet, canli.', N'alabalik,bolu,canli balik', 31, 4, NULL, 0, @now, @exp, NULL, NULL, 0, NULL, 0, 0, @now, @now),

-- 22) Cipura
(NEWID(), N'Cipura - 500 Adet - Canli - Istanbul', 'cipura-500-adet-canli-istanbul',
 N'Cipura (sea bream), 300-400 gr, 500 adetlik canli parti.',
 N'## Hayvan Bilgileri
- **Cins/Irk:** Cipura (Sea Bream)
- **Agirlik:** 300-400 gr / adet

## Konum
Istanbul, Turkiye

## Fiyat
**55 TRY / Adet** (500 adetlik parti)',
 'E1000000-0000-0000-0000-000000100103', NULL, 55, 'TRY', NULL, N'Adet', 500, N'Adet', 50, NULL, 1, @s1, @ist, 2, 0, 1, 1500, 0, 0.35, 'kg', NULL, N'Cipura - Canli - Istanbul', N'Cipura, 500 adet, canli.', N'cipura,canli balik,istanbul', 25, 3, NULL, 0, @now, @exp, NULL, NULL, 0, NULL, 0, 0, @now, @now),

-- 23) Parmak Baligi (Fingerlings)
(NEWID(), N'Tatli Su Parmak Baligi - Alabalik - 5000 Adet - Bolu', 'tatli-su-parmak-baligi-alabalik-bolu',
 N'Alabalik parmak baligi (fingerlings), 5-8 cm, 5000 adetlik parti. Ciftlik kurmak isteyenler icin.',
 N'## Urun Bilgileri
- **Tur:** Alabalik Parmak Baligi
- **Boy:** 5-8 cm

## Konum
Bolu, Turkiye

## Fiyat
**8 TRY / Adet** (5000 adetlik parti)',
 'E1000000-0000-0000-0000-000000100201', NULL, 8, 'TRY', NULL, N'Adet', 5000, N'Adet', 1000, NULL, 1, @s2, @bol, 2, 0, 1, 500, 0, 0.01, 'kg', NULL, N'Parmak Baligi - Alabalik - Bolu', N'Alabalik parmak baligi, 5000 adet.', N'parmak baligi,fingerlings,bolu', 18, 2, NULL, 0, @now, @exp, NULL, NULL, 0, NULL, 0, 0, @now, @now),

-- ═══ ARICILIK ═══
-- 24) Kovan
(NEWID(), N'Langstroth Arili Kovan - 10 Adet - Bolu', 'langstroth-arili-kovan-10-adet-bolu',
 N'Langstroth tip arili kovan, anali koloni, 10 adetlik set.',
 N'## Urun Bilgileri
- **Tip:** Langstroth Kovan
- **Durum:** Arili (anali koloni)

## Konum
Bolu, Turkiye

## Fiyat
**4.500 TRY / Kovan** (10 adet)',
 'E1000000-0000-0000-0000-000000000901', NULL, 4500, 'TRY', NULL, N'Adet', 10, N'Adet', 5, NULL, 1, @s3, @bol, 2, 0, 1, 500, 0, 25, 'kg', NULL, N'Arili Kovan - Bolu', N'Langstroth arili kovan, 10 adet.', N'kovan,aricilik,bolu', 36, 6, NULL, 0, @now, @exp, NULL, NULL, 0, NULL, 0, 0, @now, @now),

-- 25) Ana Ari
(NEWID(), N'Kafkas Ana Arisi - 20 Adet - Ankara', 'kafkas-ana-arisi-20-adet-ankara',
 N'Kafkas irki ana ari, ciflesmis, 20 adetlik parti.',
 N'## Urun Bilgileri
- **Irk:** Kafkas
- **Tip:** Ana Ari (ciflesmis)

## Konum
Ankara, Turkiye

## Fiyat
**750 TRY / Adet** (20 adet)',
 'E1000000-0000-0000-0000-000000000902', NULL, 750, 'TRY', NULL, N'Adet', 20, N'Adet', 5, NULL, 1, @s1, @ank, 2, 0, 1, 200, 0, 0.002, 'kg', NULL, N'Kafkas Ana Arisi - Ankara', N'Kafkas irki ana ari, 20 adet.', N'ana ari,kafkas,ankara', 42, 8, NULL, 0, @now, @exp, NULL, NULL, 0, NULL, 0, 0, @now, @now),

-- 26) Bal
(NEWID(), N'Organik Cicek Bali - 50 kg Teneke - Bolu', 'organik-cicek-bali-50-kg-bolu',
 N'Organik cicek bali, saf, katki maddesiz, 50 kg teneke.',
 N'## Urun Bilgileri
- **Tip:** Cicek Bali (Organik)
- **Miktar:** 50 kg teneke

## Konum
Bolu, Turkiye

## Fiyat
**450 TRY / kg** (50 kg teneke: 22.500 TRY)',
 'E1000000-0000-0000-0000-000000000903', NULL, 450, 'TRY', NULL, N'kg', 50, N'kg', 5, NULL, 1, @s2, @bol, 2, 0, 1, 300, 0, 50, 'kg', NULL, N'Organik Cicek Bali - Bolu', N'Organik cicek bali, 50 kg teneke.', N'bal,organik,bolu,cicek bali', 55, 10, NULL, 0, @now, @exp, NULL, NULL, 0, NULL, 0, 0, @now, @now),

-- ═══ TARIM MAKINALARI ═══
-- 27) Traktor
(NEWID(), N'Massey Ferguson 2635 Traktor - 2020 Model - Ankara', 'massey-ferguson-2635-traktor-ankara',
 N'Massey Ferguson 2635, 75 HP, 2020 model, 1200 saat, bakimli.',
 N'## Urun Bilgileri
- **Marka:** Massey Ferguson
- **Model:** 2635
- **Guc:** 75 HP
- **Yil:** 2020
- **Calisma Saati:** 1200 saat
- **Kondisyon:** Ikinci el (bakimli)

## Konum
Ankara, Turkiye

## Fiyat
**850.000 TRY**',
 'E1000000-0000-0000-0000-000000000601', NULL, 850000, 'TRY', NULL, N'Adet', 1, N'Adet', NULL, NULL, 1, @s3, @ank, 2, 1, 0, NULL, 0, 2800, 'kg', NULL, N'Massey Ferguson 2635 Traktor - Ankara', N'Massey Ferguson 2635, 75 HP, 2020 model.', N'traktor,massey ferguson,ankara', 93, 16, NULL, 0, @now, @exp, NULL, NULL, 1, DATEADD(DAY,30,@now), 4, 0, @now, @now),

-- 28) Sagim Makinesi
(NEWID(), N'Sezer 4 Baslik Sagim Makinesi - Sifir - Istanbul', 'sezer-4-baslik-sagim-makinesi-istanbul',
 N'Sezer marka, 4 baslik, sabit tip sagim makinesi. Sifir urun.',
 N'## Urun Bilgileri
- **Marka:** Sezer
- **Tip:** 4 Baslik Sabit Sagim
- **Kondisyon:** Sifir/Yeni

## Konum
Istanbul, Turkiye

## Fiyat
**125.000 TRY**',
 'E1000000-0000-0000-0000-000000000606', NULL, 125000, 'TRY', NULL, N'Adet', 1, N'Adet', NULL, NULL, 1, @s1, @ist, 2, 0, 1, 2000, 0, 150, 'kg', NULL, N'Sagim Makinesi 4 Baslik - Istanbul', N'Sezer 4 baslik sabit sagim makinesi.', N'sagim makinesi,sezer,istanbul', 47, 8, NULL, 0, @now, @exp, NULL, NULL, 0, NULL, 0, 0, @now, @now),

-- 29) Sulama Sistemi
(NEWID(), N'Damlama Sulama Sistemi - 10 Donum Set - Ankara', 'damlama-sulama-sistemi-10-donum-ankara',
 N'Damlama sulama sistemi komple set, 10 donumluk alan icin.',
 N'## Urun Bilgileri
- **Tip:** Damlama Sulama
- **Kapasite:** 10 donum
- **Icerik:** Boru, damlatiici, filtre, vana dahil

## Konum
Ankara, Turkiye

## Fiyat
**45.000 TRY / Set**',
 'E1000000-0000-0000-0000-000000000604', NULL, 45000, 'TRY', NULL, N'Set', 1, N'Set', NULL, NULL, 1, @s2, @ank, 2, 0, 1, 1500, 0, 200, 'kg', NULL, N'Damlama Sulama Sistemi - Ankara', N'Damlama sulama sistemi, 10 donum set.', N'sulama,damlama,ankara', 31, 5, NULL, 0, @now, @exp, NULL, NULL, 0, NULL, 0, 0, @now, @now),

-- 30) Hayvan Tartisi
(NEWID(), N'Dijital Buyukbas Hayvan Tartisi - 2000 kg - Istanbul', 'dijital-hayvan-tartisi-2000kg-istanbul',
 N'Dijital buyukbas hayvan tartisi, 2000 kg kapasiteli, paslanmaz celik.',
 N'## Urun Bilgileri
- **Tip:** Dijital Platform Tarti
- **Kapasite:** 2000 kg
- **Malzeme:** Paslanmaz Celik

## Konum
Istanbul, Turkiye

## Fiyat
**65.000 TRY**',
 'E1000000-0000-0000-0000-000000000605', NULL, 65000, 'TRY', NULL, N'Adet', 1, N'Adet', NULL, NULL, 1, @s3, @ist, 2, 0, 1, 3000, 0, 350, 'kg', NULL, N'Hayvan Tartisi 2000kg - Istanbul', N'Dijital buyukbas hayvan tartisi, 2000 kg.', N'hayvan tartisi,dijital,istanbul', 22, 3, NULL, 0, @now, @exp, NULL, NULL, 0, NULL, 0, 0, @now, @now),

-- ═══ YEM ve YEMLIK ═══
-- 31) Saman
(NEWID(), N'Bugday Samani - 500 Balya - Ankara', 'bugday-samani-500-balya-ankara',
 N'Bugday samani, kuru, kaliteli, 500 balya (25 kg/balya).',
 N'## Urun Bilgileri
- **Tip:** Bugday Samani
- **Miktar:** 500 balya
- **Balya Agirligi:** 25 kg

## Konum
Ankara, Turkiye

## Fiyat
**120 TRY / Balya** (500 balya)',
 'E1000000-0000-0000-0000-000000000502', NULL, 120, 'TRY', NULL, N'Balya', 500, N'Balya', 50, NULL, 1, @s1, @ank, 2, 0, 1, 0, 0, 25, 'kg', NULL, N'Bugday Samani - 500 Balya - Ankara', N'Bugday samani, 500 balya, kaliteli.', N'saman,bugday,ankara', 38, 5, NULL, 0, @now, @exp, NULL, NULL, 0, NULL, 0, 0, @now, @now),

-- 32) Yonca
(NEWID(), N'Kuru Yonca - 200 Balya - Bolu', 'kuru-yonca-200-balya-bolu',
 N'Kuru yonca (alfalfa), protein orani yuksek, 200 balya.',
 N'## Urun Bilgileri
- **Tip:** Kuru Yonca (Alfalfa)
- **Miktar:** 200 balya
- **Protein Orani:** %18+

## Konum
Bolu, Turkiye

## Fiyat
**250 TRY / Balya** (200 balya)',
 'E1000000-0000-0000-0000-000000000505', NULL, 250, 'TRY', NULL, N'Balya', 200, N'Balya', 20, NULL, 1, @s2, @bol, 2, 0, 1, 0, 0, 30, 'kg', NULL, N'Kuru Yonca - 200 Balya - Bolu', N'Kuru yonca, protein orani yuksek.', N'yonca,alfalfa,bolu', 27, 4, NULL, 0, @now, @exp, NULL, NULL, 0, NULL, 0, 0, @now, @now),

-- 33) Buyukbas Besi Yemi
(NEWID(), N'Buyukbas Besi Yemi - 5 Ton - Istanbul', 'buyukbas-besi-yemi-5-ton-istanbul',
 N'Buyukbas besi yemi, %16 protein, 5 tonluk parti.',
 N'## Urun Bilgileri
- **Tip:** Buyukbas Besi Yemi
- **Protein:** %16
- **Miktar:** 5 ton (200 cuval x 25 kg)

## Konum
Istanbul, Turkiye

## Fiyat
**18 TRY / kg** (5 ton)',
 'E1000000-0000-0000-0000-000000000501', NULL, 18, 'TRY', NULL, N'kg', 5000, N'kg', 500, NULL, 1, @s3, @ist, 2, 0, 1, 0, 0, 25, 'kg', NULL, N'Buyukbas Besi Yemi - Istanbul', N'Buyukbas besi yemi, %16 protein, 5 ton.', N'besi yemi,buyukbas,istanbul', 44, 7, NULL, 0, @now, @exp, NULL, NULL, 0, NULL, 0, 0, @now, @now),

-- 34) Vitamin Takviyesi
(NEWID(), N'Buyukbas Vitamin-Mineral Premiks - 25 kg - Ankara', 'buyukbas-vitamin-premiks-25kg-ankara',
 N'Buyukbas icin vitamin ve mineral premiksi, 25 kg cuval.',
 N'## Urun Bilgileri
- **Tip:** Vitamin-Mineral Premiksi
- **Miktar:** 25 kg
- **Kullanim:** Yeme karistirma

## Konum
Ankara, Turkiye

## Fiyat
**1.200 TRY / 25 kg**',
 'E1000000-0000-0000-0000-000000000504', NULL, 1200, 'TRY', NULL, N'Cuval', 20, N'Cuval', 1, NULL, 1, @s1, @ank, 2, 0, 1, 300, 0, 25, 'kg', NULL, N'Vitamin Premiks - Buyukbas - Ankara', N'Buyukbas vitamin-mineral premiksi.', N'vitamin,premiks,buyukbas,ankara', 16, 2, NULL, 0, @now, @exp, NULL, NULL, 0, NULL, 0, 0, @now, @now),

-- ═══ VETERINERLIK ═══
-- 35) Hayvan Asisi
(NEWID(), N'Sap Hastaligi Asisi - 50 Dozluk - Istanbul', 'sap-hastaligi-asisi-50-dozluk-istanbul',
 N'Sap hastaligi (FMD) asisi, buyukbas icin, 50 dozluk sise.',
 N'## Urun Bilgileri
- **Tip:** Sap Hastaligi (FMD) Asisi
- **Miktar:** 50 doz / sise
- **Hedef:** Buyukbas Hayvan

## Konum
Istanbul, Turkiye

## Fiyat
**850 TRY / Sise (50 doz)**',
 'E1000000-0000-0000-0000-000000000702', NULL, 850, 'TRY', NULL, N'Sise', 10, N'Sise', 1, NULL, 1, @s2, @ist, 2, 0, 1, 200, 0, 0.5, 'kg', NULL, N'Sap Hastaligi Asisi - Istanbul', N'Sap hastaligi asisi, 50 dozluk.', N'asi,sap hastaligi,fmd,istanbul', 33, 5, NULL, 0, @now, @exp, NULL, NULL, 0, NULL, 0, 0, @now, @now),

-- 36) Antiparaziter
(NEWID(), N'Ivermektin Antiparaziter - 500 ml - Ankara', 'ivermektin-antiparaziter-500ml-ankara',
 N'Ivermektin bazli antiparaziter ilac, buyukbas ve kucukbas icin, 500 ml.',
 N'## Urun Bilgileri
- **Etken Madde:** Ivermektin
- **Miktar:** 500 ml
- **Kullanim:** Buyukbas ve Kucukbas

## Konum
Ankara, Turkiye

## Fiyat
**650 TRY / Sise**',
 'E1000000-0000-0000-0000-000000000705', NULL, 650, 'TRY', NULL, N'Sise', 15, N'Sise', 1, NULL, 1, @s3, @ank, 2, 0, 1, 100, 0, 0.5, 'kg', NULL, N'Ivermektin Antiparaziter - Ankara', N'Ivermektin antiparaziter, 500 ml.', N'ivermektin,antiparaziter,ankara', 21, 3, NULL, 0, @now, @exp, NULL, NULL, 0, NULL, 0, 0, @now, @now),

-- ═══ TOHUM ve FIDE ═══
-- 37) Misir Tohumu
(NEWID(), N'Hibrit Misir Tohumu - 50 kg - Ankara', 'hibrit-misir-tohumu-50kg-ankara',
 N'Hibrit silajlik misir tohumu, yuksek verimli, 50 kg cuval.',
 N'## Urun Bilgileri
- **Tip:** Hibrit Misir Tohumu
- **Amac:** Silajlik
- **Miktar:** 50 kg

## Konum
Ankara, Turkiye

## Fiyat
**3.500 TRY / 50 kg**',
 'E1000000-0000-0000-0000-000000000801', NULL, 3500, 'TRY', NULL, N'Cuval', 20, N'Cuval', 1, NULL, 1, @s1, @ank, 2, 0, 1, 500, 0, 50, 'kg', NULL, N'Hibrit Misir Tohumu - Ankara', N'Hibrit silajlik misir tohumu, 50 kg.', N'misir tohumu,hibrit,ankara', 29, 4, NULL, 0, @now, @exp, NULL, NULL, 0, NULL, 0, 0, @now, @now),

-- 38) Mera Tohumu
(NEWID(), N'Mera Karisimi Tohumu - 25 kg - Bolu', 'mera-karisimi-tohumu-25kg-bolu',
 N'Mera karisimi (yonca + cayir otlari), otlatma alanlari icin, 25 kg.',
 N'## Urun Bilgileri
- **Tip:** Mera Karisimi
- **Icerik:** Yonca + Cayir Otlari
- **Miktar:** 25 kg

## Konum
Bolu, Turkiye

## Fiyat
**2.200 TRY / 25 kg**',
 'E1000000-0000-0000-0000-000000000804', NULL, 2200, 'TRY', NULL, N'Cuval', 30, N'Cuval', 1, NULL, 1, @s2, @bol, 2, 0, 1, 300, 0, 25, 'kg', NULL, N'Mera Tohumu Karisimi - Bolu', N'Mera karisimi tohumu, 25 kg.', N'mera tohumu,cayir,bolu', 18, 2, NULL, 0, @now, @exp, NULL, NULL, 0, NULL, 0, 0, @now, @now),

-- 39) Meyve Fidani
(NEWID(), N'Ceviz Fidani (Chandler) - 100 Adet - Bolu', 'ceviz-fidani-chandler-100-adet-bolu',
 N'Chandler cesidi ceviz fidani, 2 yasinda, asili, 100 adet.',
 N'## Urun Bilgileri
- **Cesit:** Chandler Ceviz
- **Yas:** 2 yasinda asili fidan
- **Miktar:** 100 adet

## Konum
Bolu, Turkiye

## Fiyat
**350 TRY / Adet** (100 adet)',
 'E1000000-0000-0000-0000-000000000803', NULL, 350, 'TRY', NULL, N'Adet', 100, N'Adet', 10, NULL, 1, @s3, @bol, 2, 0, 1, 500, 0, 2, 'kg', NULL, N'Ceviz Fidani Chandler - Bolu', N'Chandler ceviz fidani, 100 adet.', N'ceviz fidani,chandler,bolu', 37, 6, NULL, 0, @now, @exp, NULL, NULL, 0, NULL, 0, 0, @now, @now),

-- ═══ CIFTLIK ALTYAPI ═══
-- 40) Panel Cit
(NEWID(), N'Galvaniz Panel Cit - 100 mt Set - Istanbul', 'galvaniz-panel-cit-100mt-istanbul',
 N'Galvanizli panel cit, 150 cm yukseklik, 100 metrelik set, direkleri dahil.',
 N'## Urun Bilgileri
- **Tip:** Galvaniz Panel Cit
- **Yukseklik:** 150 cm
- **Uzunluk:** 100 metre
- **Icerik:** Panel + Direk + Baglanti Malzemesi

## Konum
Istanbul, Turkiye

## Fiyat
**75.000 TRY / 100 mt Set**',
 'E1000000-0000-0000-0000-000000001201', NULL, 75000, 'TRY', NULL, N'Set', 1, N'Set', NULL, NULL, 1, @s1, @ist, 2, 0, 1, 5000, 0, 2000, 'kg', NULL, N'Galvaniz Panel Cit - Istanbul', N'Galvaniz panel cit, 100 mt set.', N'panel cit,galvaniz,istanbul', 41, 7, NULL, 0, @now, @exp, NULL, NULL, 0, NULL, 0, 0, @now, @now),

-- 41) Yemlik Suluk
(NEWID(), N'Otomatik Paslanmaz Suluk - Buyukbas - 20 Adet - Ankara', 'otomatik-suluk-buyukbas-20-adet-ankara',
 N'Otomatik paslanmaz celik suluk, buyukbas icin, 20 adetlik set.',
 N'## Urun Bilgileri
- **Tip:** Otomatik Suluk
- **Malzeme:** Paslanmaz Celik
- **Kullanim:** Buyukbas
- **Miktar:** 20 adet

## Konum
Ankara, Turkiye

## Fiyat
**850 TRY / Adet** (20 adet)',
 'E1000000-0000-0000-0000-000000001203', NULL, 850, 'TRY', NULL, N'Adet', 20, N'Adet', 5, NULL, 1, @s2, @ank, 2, 0, 1, 500, 0, 5, 'kg', NULL, N'Otomatik Suluk - Buyukbas - Ankara', N'Paslanmaz otomatik suluk, 20 adet.', N'suluk,otomatik,buyukbas,ankara', 19, 3, NULL, 0, @now, @exp, NULL, NULL, 0, NULL, 0, 0, @now, @now),

-- ═══ DAHA FAZLA KANATLI ═══
-- 42) Leghorn Tavuk
(NEWID(), N'Leghorn Yumurta Tavugu - 100 Adet - Pendik', 'leghorn-yumurta-tavugu-100-adet-pendik',
 N'Leghorn cinsi beyaz yumurta tavugu, 8 aylik, 100 adetlik parti.',
 N'## Hayvan Bilgileri
- **Cins/Irk:** Leghorn
- **Yas:** 8 ay
- **Amac:** Yumurta Uretimi

## Konum
Pendik, Istanbul, Turkiye

## Fiyat
**300 TRY / Adet** (100 adet)',
 'E1000000-0000-0000-0000-000000030101', NULL, 300, 'TRY', NULL, N'Adet', 100, N'Adet', 20, NULL, 1, @s3, @pen, 2, 0, 1, 500, 0, 1.8, 'kg', NULL, N'Leghorn Yumurta Tavugu - Pendik', N'Leghorn cinsi, 100 adet.', N'leghorn,tavuk,yumurta,pendik', 24, 4, NULL, 0, @now, @exp, NULL, NULL, 0, NULL, 0, 0, @now, @now),

-- 43) Rhode Island Red
(NEWID(), N'Rhode Island Red Tavuk - 30 Adet - Bolu', 'rhode-island-red-tavuk-30-adet-bolu',
 N'Rhode Island Red cinsi, 6 aylik, hem yumurta hem et icin, 30 adet.',
 N'## Hayvan Bilgileri
- **Cins/Irk:** Rhode Island Red
- **Yas:** 6 ay
- **Amac:** Cift amacli (Yumurta + Et)

## Konum
Bolu, Turkiye

## Fiyat
**400 TRY / Adet** (30 adet)',
 'E1000000-0000-0000-0000-000000030102', NULL, 400, 'TRY', NULL, N'Adet', 30, N'Adet', 5, NULL, 1, @s1, @bol, 2, 0, 1, 500, 0, 2.5, 'kg', NULL, N'Rhode Island Red Tavuk - Bolu', N'Rhode Island Red cinsi, 30 adet.', N'rhode island red,tavuk,bolu', 20, 3, NULL, 0, @now, @exp, NULL, NULL, 0, NULL, 0, 0, @now, @now),

-- 44) Genis Goguslu Beyaz Hindi
(NEWID(), N'Genis Goguslu Beyaz Hindi - 20 Adet - Umraniye', 'genis-goguslu-beyaz-hindi-20-adet-umraniye',
 N'Genis Goguslu Beyaz Hindi, 4 aylik, 20 adetlik parti.',
 N'## Hayvan Bilgileri
- **Cins/Irk:** Genis Goguslu Beyaz Hindi
- **Yas:** 4 ay
- **Agirlik:** 6-8 kg

## Konum
Umraniye, Istanbul, Turkiye

## Fiyat
**1.500 TRY / Adet** (20 adet)',
 'E1000000-0000-0000-0000-000000030301', NULL, 1500, 'TRY', NULL, N'Adet', 20, N'Adet', 5, NULL, 1, @s2, @umr, 2, 0, 1, 800, 0, 7, 'kg', NULL, N'Beyaz Hindi - 20 Adet - Umraniye', N'Genis Goguslu Beyaz Hindi, 20 adet.', N'hindi,beyaz,umraniye', 28, 4, NULL, 0, @now, @exp, NULL, NULL, 0, NULL, 0, 0, @now, @now),

-- 45) Pekin Ordegi
(NEWID(), N'Pekin Ordegi - Cift - Istanbul', 'pekin-ordegi-cift-istanbul',
 N'Pekin ordegi cifti (1 erkek + 1 disi), 6 aylik.',
 N'## Hayvan Bilgileri
- **Cins/Irk:** Pekin Ordegi
- **Yas:** 6 ay

## Konum
Istanbul, Turkiye

## Fiyat
**1.200 TRY / Cift**',
 'E1000000-0000-0000-0000-000000030401', NULL, 1200, 'TRY', NULL, N'Cift', 5, N'Cift', 1, NULL, 1, @s3, @ist, 2, 0, 1, 500, 0, 3.5, 'kg', NULL, N'Pekin Ordegi - Istanbul', N'Pekin ordegi cifti.', N'pekin,ordek,istanbul', 17, 2, NULL, 0, @now, @exp, NULL, NULL, 0, NULL, 0, 0, @now, @now),

-- 46) Civciv
(NEWID(), N'ISA Brown Civciv - 200 Adet - Ankara', 'isa-brown-civciv-200-adet-ankara',
 N'ISA Brown cinsi civciv, 1 gunluk, yumurtaci, 200 adetlik parti.',
 N'## Hayvan Bilgileri
- **Cins/Irk:** ISA Brown
- **Yas:** 1 gunluk
- **Amac:** Yumurta Uretimi

## Konum
Ankara, Turkiye

## Fiyat
**30 TRY / Adet** (200 adet)',
 'E1000000-0000-0000-0000-000000000305', NULL, 30, 'TRY', NULL, N'Adet', 200, N'Adet', 50, NULL, 1, @s1, @ank, 2, 0, 1, 300, 0, 0.04, 'kg', NULL, N'ISA Brown Civciv - 200 Adet - Ankara', N'ISA Brown civciv, 1 gunluk, 200 adet.', N'civciv,isa brown,ankara', 35, 6, NULL, 0, @now, @exp, NULL, NULL, 0, NULL, 0, 0, @now, @now),

-- ═══ DAHA FAZLA TAVSAN ═══
-- 47) Yeni Zelanda Beyazi
(NEWID(), N'Yeni Zelanda Beyazi Tavsan - 10 Adet - Pendik', 'yeni-zelanda-beyazi-tavsan-pendik',
 N'Yeni Zelanda Beyazi cinsi tavsan, 4 aylik, et irki, 10 adetlik parti.',
 N'## Hayvan Bilgileri
- **Cins/Irk:** Yeni Zelanda Beyazi
- **Yas:** 4 ay
- **Amac:** Et Uretimi

## Konum
Pendik, Istanbul, Turkiye

## Fiyat
**500 TRY / Adet** (10 adet)',
 'E1000000-0000-0000-0000-000000110101', NULL, 500, 'TRY', NULL, N'Adet', 10, N'Adet', 3, NULL, 1, @s2, @pen, 2, 0, 1, 300, 0, 4, 'kg', NULL, N'Yeni Zelanda Tavsan - Pendik', N'Yeni Zelanda Beyazi tavsan, 10 adet.', N'tavsan,yeni zelanda,pendik', 15, 2, NULL, 0, @now, @exp, NULL, NULL, 0, NULL, 0, 0, @now, @now),

-- ═══ EK KOYUN / KUZU ═══
-- 48) Besi Kuzusu
(NEWID(), N'Besi Kuzusu - Karisik Irk - 20 Baslik - Istanbul', 'besi-kuzusu-karisik-20-baslik-istanbul',
 N'Karisik irk besi kuzusu, 3-4 aylik, 20 baslik parti.',
 N'## Hayvan Bilgileri
- **Yas:** 3-4 ay
- **Agirlik:** 20-28 kg
- **Amac:** Besicilik

## Konum
Istanbul, Turkiye

## Fiyat
**7.000 TRY / Bas** (20 baslik)',
 'E1000000-0000-0000-0000-000000020302', NULL, 7000, 'TRY', NULL, N'Bas', 20, N'Bas', 10, NULL, 1, @s3, @ist, 2, 0, 1, 2000, 0, 24, 'kg', NULL, N'Besi Kuzusu - 20 Baslik - Istanbul', N'Besi kuzusu, 20 baslik parti.', N'besi kuzusu,istanbul', 40, 7, NULL, 0, @now, @exp, NULL, NULL, 0, NULL, 0, 0, @now, @now),

-- 49) Daglic Koyun
(NEWID(), N'Daglic Koyun - 8 Baslik Suru - Bolu', 'daglic-koyun-8-baslik-suru-bolu',
 N'Daglic cinsi koyun, daglik araziye dayanikli, 8 baslik suru.',
 N'## Hayvan Bilgileri
- **Cins/Irk:** Daglic
- **Yas:** 2-3 yas
- **Agirlik:** 50-60 kg

## Konum
Bolu, Turkiye

## Fiyat
**11.000 TRY / Bas** (8 baslik)',
 'E1000000-0000-0000-0000-000000020110', NULL, 11000, 'TRY', NULL, N'Bas', 8, N'Bas', 4, NULL, 1, @s1, @bol, 2, 0, 1, 1500, 0, 55, 'kg', NULL, N'Daglic Koyun - 8 Baslik - Bolu', N'Daglic cinsi koyun, 8 baslik suru.', N'daglic,koyun,bolu', 22, 3, NULL, 0, @now, @exp, NULL, NULL, 0, NULL, 0, 0, @now, @now),

-- 50) Silaj
(NEWID(), N'Misir Silaji - 50 Ton - Ankara', 'misir-silaji-50-ton-ankara',
 N'Misir silaji, yuksek enerji, buyukbas besi icin ideal, 50 tonluk parti.',
 N'## Urun Bilgileri
- **Tip:** Misir Silaji
- **Miktar:** 50 ton
- **Kuru Madde:** %30-35

## Konum
Ankara, Turkiye

## Fiyat
**5 TRY / kg** (50 ton)',
 'E1000000-0000-0000-0000-000000000503', NULL, 5, 'TRY', NULL, N'kg', 50000, N'kg', 1000, NULL, 1, @s2, @ank, 2, 0, 1, 0, 0, 50000, 'kg', NULL, N'Misir Silaji - 50 Ton - Ankara', N'Misir silaji, 50 ton, buyukbas besi icin.', N'silaj,misir,ankara,besi', 33, 5, NULL, 0, @now, @exp, NULL, NULL, 0, NULL, 0, 0, @now, @now);

SELECT COUNT(*) AS ToplamAktifIlan FROM Products WHERE IsDeleted = 0 AND Status = 2;

COMMIT TRANSACTION;
GO
