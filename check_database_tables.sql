-- ================================================
-- VERİTABANI KONTROL SCRİPTİ
-- Bu script bağlı olduğunuz veritabanını ve tabloları gösterir
-- ================================================

PRINT '================================================';
PRINT 'Veritabanı Kontrol Scripti';
PRINT '================================================';
PRINT '';

-- 1. Bağlı olduğunuz veritabanını göster
PRINT '1. BAĞLI OLDUĞUNUZ VERİTABANI:';
SELECT DB_NAME() AS [Current Database];
PRINT '';

-- 2. Tüm tabloları listele
PRINT '2. VERİTABANINDAKI TÜM TABLOLAR:';
SELECT
    SCHEMA_NAME(schema_id) AS [Schema],
    name AS [Table Name],
    OBJECT_ID AS [Object ID]
FROM sys.tables
ORDER BY SCHEMA_NAME(schema_id), name;
PRINT '';

-- 3. User ile ilgili tabloları ara
PRINT '3. "USER" KELİMESİNİ İÇEREN TABLOLAR:';
SELECT
    SCHEMA_NAME(schema_id) AS [Schema],
    name AS [Table Name]
FROM sys.tables
WHERE name LIKE '%User%' OR name LIKE '%user%'
ORDER BY name;
PRINT '';

-- 4. Animal ile ilgili tabloları ara
PRINT '4. "ANIMAL" KELİMESİNİ İÇEREN TABLOLAR:';
SELECT
    SCHEMA_NAME(schema_id) AS [Schema],
    name AS [Table Name]
FROM sys.tables
WHERE name LIKE '%Animal%' OR name LIKE '%animal%'
ORDER BY name;
PRINT '';

-- 5. Hirovo ile ilgili tabloları ara
PRINT '5. "HIROVO" KELİMESİNİ İÇEREN TABLOLAR:';
SELECT
    SCHEMA_NAME(schema_id) AS [Schema],
    name AS [Table Name]
FROM sys.tables
WHERE name LIKE '%Hirovo%' OR name LIKE '%hirovo%'
ORDER BY name;
PRINT '';

-- 6. App ile başlayan tabloları ara
PRINT '6. "APP" İLE BAŞLAYAN TABLOLAR:';
SELECT
    SCHEMA_NAME(schema_id) AS [Schema],
    name AS [Table Name]
FROM sys.tables
WHERE name LIKE 'App%' OR name LIKE 'app%'
ORDER BY name;
PRINT '';

-- 7. Tüm veritabanlarını listele (erişiminiz varsa)
PRINT '7. SUNUCUDAKI TÜM VERİTABANLARI:';
SELECT
    name AS [Database Name],
    database_id AS [Database ID],
    create_date AS [Created Date]
FROM sys.databases
ORDER BY name;
PRINT '';

PRINT '================================================';
PRINT 'Kontrol tamamlandı!';
PRINT '================================================';

GO
