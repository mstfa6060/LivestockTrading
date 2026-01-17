-- ================================================
-- KULLANICI SOFT DELETE SCRIPTI (IsDeleted flag ile)
-- Bu script bir kullanıcıyı ve tüm ilişkili verilerini IsDeleted=1 yaparak "yumuşak siler"
-- Veritabanından fiziksel olarak silinmez, sadece işaretlenir
-- ================================================

-- KULLANIM:
-- @userId parametresini silmek istediğiniz kullanıcının GUID'i ile değiştirin
-- ÖRNEK: DECLARE @userId UNIQUEIDENTIFIER = 'XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX';

DECLARE @userId UNIQUEIDENTIFIER = 'KULLANICI_ID_BURAYA';
DECLARE @deletedAt DATETIME2 = GETUTCDATE();

-- İşlem başlat (hata durumunda geri alabilmek için)
BEGIN TRANSACTION;

BEGIN TRY
    PRINT '================================================';
    PRINT 'Kullanıcı soft delete işlemi başlıyor...';
    PRINT 'User ID: ' + CAST(@userId AS NVARCHAR(50));
    PRINT 'Deleted At: ' + CAST(@deletedAt AS NVARCHAR(50));
    PRINT '================================================';

    -- ================================================
    -- 1. HIROVO MODÜLÜ - İş ilanları ve başvurular
    -- ================================================

    PRINT 'Hirovo modülü soft delete yapılıyor...';

    -- HirovoJobApplications tablosunda WorkerId ile ilişkili kayıtları işaretle
    IF OBJECT_ID('dbo.HirovoJobApplications', 'U') IS NOT NULL
    BEGIN
        UPDATE HirovoJobApplications
        SET IsDeleted = 1, DeletedAt = @deletedAt
        WHERE WorkerId = @userId AND IsDeleted = 0;
        PRINT '  - HirovoJobApplications soft deleted (WorkerId)';
    END
    ELSE
        PRINT '  - HirovoJobApplications tablosu bulunamadı (atlanıyor)';

    -- HirovoJobs tablosunda HirovoEmployerId ile ilişkili kayıtları işaretle
    IF OBJECT_ID('dbo.HirovoJobs', 'U') IS NOT NULL
    BEGIN
        UPDATE HirovoJobs
        SET IsDeleted = 1, DeletedAt = @deletedAt
        WHERE HirovoEmployerId = @userId AND IsDeleted = 0;
        PRINT '  - HirovoJobs soft deleted (HirovoEmployerId)';
    END
    ELSE
        PRINT '  - HirovoJobs tablosu bulunamadı (atlanıyor)';

    -- HirovoSubscriptions tablosunda UserId ile ilişkili kayıtları işaretle
    IF OBJECT_ID('dbo.HirovoSubscriptions', 'U') IS NOT NULL
        AND EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('HirovoSubscriptions') AND name = 'IsDeleted')
    BEGIN
        UPDATE HirovoSubscriptions
        SET IsDeleted = 1, DeletedAt = @deletedAt
        WHERE UserId = @userId AND IsDeleted = 0;
        PRINT '  - HirovoSubscriptions soft deleted';
    END
    ELSE
        PRINT '  - HirovoSubscriptions tablosu bulunamadı veya IsDeleted yok (atlanıyor)';

    -- HirovoFileAccessCodes tablosunda GeneratorUserId ile ilişkili kayıtları işaretle
    IF OBJECT_ID('dbo.HirovoFileAccessCodes', 'U') IS NOT NULL
        AND EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('HirovoFileAccessCodes') AND name = 'IsDeleted')
    BEGIN
        UPDATE HirovoFileAccessCodes
        SET IsDeleted = 1, DeletedAt = @deletedAt
        WHERE GeneratorUserId = @userId AND IsDeleted = 0;
        PRINT '  - HirovoFileAccessCodes soft deleted';
    END
    ELSE
        PRINT '  - HirovoFileAccessCodes tablosu bulunamadı veya IsDeleted yok (atlanıyor)';

    -- HirovoDocumentRoots tablosunda CreatorUserId ve ManualCreatorUserId ile ilişkili kayıtları işaretle
    IF OBJECT_ID('dbo.HirovoDocumentRoots', 'U') IS NOT NULL
        AND EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('HirovoDocumentRoots') AND name = 'IsDeleted')
    BEGIN
        UPDATE HirovoDocumentRoots
        SET IsDeleted = 1, DeletedAt = @deletedAt
        WHERE (CreatorUserId = @userId OR ManualCreatorUserId = @userId) AND IsDeleted = 0;
        PRINT '  - HirovoDocumentRoots soft deleted';
    END
    ELSE
        PRINT '  - HirovoDocumentRoots tablosu bulunamadı veya IsDeleted yok (atlanıyor)';

    -- ================================================
    -- 2. ANIMAL MARKET MODÜLÜ - Hayvan pazarı
    -- ================================================

    PRINT 'AnimalMarket modülü soft delete yapılıyor...';

    -- AnimalMarket_FarmUsers tablosunda UserId ile ilişkili kayıtları işaretle
    IF OBJECT_ID('dbo.AnimalMarket_FarmUsers', 'U') IS NOT NULL
    BEGIN
        UPDATE AnimalMarket_FarmUsers
        SET IsDeleted = 1, DeletedAt = @deletedAt
        WHERE UserId = @userId AND IsDeleted = 0;
        PRINT '  - AnimalMarket_FarmUsers soft deleted';
    END
    ELSE
        PRINT '  - AnimalMarket_FarmUsers tablosu bulunamadı (atlanıyor)';

    -- AnimalMarket_VeterinaryApprovals tablosunda VeterinarianId ile ilişkili kayıtları işaretle
    IF OBJECT_ID('dbo.AnimalMarket_VeterinaryApprovals', 'U') IS NOT NULL
    BEGIN
        UPDATE AnimalMarket_VeterinaryApprovals
        SET IsDeleted = 1, DeletedAt = @deletedAt
        WHERE VeterinarianId = @userId AND IsDeleted = 0;
        PRINT '  - AnimalMarket_VeterinaryApprovals soft deleted';
    END
    ELSE
        PRINT '  - AnimalMarket_VeterinaryApprovals tablosu bulunamadı (atlanıyor)';

    -- AnimalMarket_DeliveryOrders tablosunda CarrierId ve BuyerId ile ilişkili kayıtları işaretle
    IF OBJECT_ID('dbo.AnimalMarket_DeliveryOrders', 'U') IS NOT NULL
    BEGIN
        UPDATE AnimalMarket_DeliveryOrders
        SET IsDeleted = 1, DeletedAt = @deletedAt
        WHERE (CarrierId = @userId OR BuyerId = @userId) AND IsDeleted = 0;
        PRINT '  - AnimalMarket_DeliveryOrders soft deleted';
    END
    ELSE
        PRINT '  - AnimalMarket_DeliveryOrders tablosu bulunamadı (atlanıyor)';

    -- AnimalMarket_AnimalBids tablosunda BidderId ile ilişkili kayıtları işaretle
    IF OBJECT_ID('dbo.AnimalMarket_AnimalBids', 'U') IS NOT NULL
    BEGIN
        UPDATE AnimalMarket_AnimalBids
        SET IsDeleted = 1, DeletedAt = @deletedAt
        WHERE BidderId = @userId AND IsDeleted = 0;
        PRINT '  - AnimalMarket_AnimalBids soft deleted';
    END
    ELSE
        PRINT '  - AnimalMarket_AnimalBids tablosu bulunamadı (atlanıyor)';

    -- AnimalMarket_AnimalOffers tablosunda BuyerId ile ilişkili kayıtları işaretle
    IF OBJECT_ID('dbo.AnimalMarket_AnimalOffers', 'U') IS NOT NULL
    BEGIN
        UPDATE AnimalMarket_AnimalOffers
        SET IsDeleted = 1, DeletedAt = @deletedAt
        WHERE BuyerId = @userId AND IsDeleted = 0;
        PRINT '  - AnimalMarket_AnimalOffers soft deleted';
    END
    ELSE
        PRINT '  - AnimalMarket_AnimalOffers tablosu bulunamadı (atlanıyor)';

    -- AnimalMarket_EscrowTransactions tablosunda BuyerId ve SellerId ile ilişkili kayıtları işaretle
    IF OBJECT_ID('dbo.AnimalMarket_EscrowTransactions', 'U') IS NOT NULL
    BEGIN
        UPDATE AnimalMarket_EscrowTransactions
        SET IsDeleted = 1, DeletedAt = @deletedAt
        WHERE (BuyerId = @userId OR SellerId = @userId) AND IsDeleted = 0;
        PRINT '  - AnimalMarket_EscrowTransactions soft deleted';
    END
    ELSE
        PRINT '  - AnimalMarket_EscrowTransactions tablosu bulunamadı (atlanıyor)';

    -- AnimalMarket_PaymentTransactions tablosunda PayerId ve PayeeId ile ilişkili kayıtları işaretle
    IF OBJECT_ID('dbo.AnimalMarket_PaymentTransactions', 'U') IS NOT NULL
    BEGIN
        UPDATE AnimalMarket_PaymentTransactions
        SET IsDeleted = 1, DeletedAt = @deletedAt
        WHERE (PayerId = @userId OR PayeeId = @userId) AND IsDeleted = 0;
        PRINT '  - AnimalMarket_PaymentTransactions soft deleted (PayerId, PayeeId)';
    END
    ELSE
        PRINT '  - AnimalMarket_PaymentTransactions tablosu bulunamadı (atlanıyor)';

    -- AnimalMarket_Animals tablosunda SellerId ile ilişkili kayıtları işaretle
    IF OBJECT_ID('dbo.AnimalMarket_Animals', 'U') IS NOT NULL
    BEGIN
        UPDATE AnimalMarket_Animals
        SET IsDeleted = 1, DeletedAt = @deletedAt
        WHERE SellerId = @userId AND IsDeleted = 0;
        PRINT '  - AnimalMarket_Animals soft deleted (SellerId)';
    END
    ELSE
        PRINT '  - AnimalMarket_Animals tablosu bulunamadı (atlanıyor)';

    -- AnimalMarket_Farms tablosunda OwnerId ile ilişkili kayıtları işaretle
    IF OBJECT_ID('dbo.AnimalMarket_Farms', 'U') IS NOT NULL
    BEGIN
        UPDATE AnimalMarket_Farms
        SET IsDeleted = 1, DeletedAt = @deletedAt
        WHERE OwnerId = @userId AND IsDeleted = 0;
        PRINT '  - AnimalMarket_Farms soft deleted (OwnerId)';
    END
    ELSE
        PRINT '  - AnimalMarket_Farms tablosu bulunamadı (atlanıyor)';

    -- ================================================
    -- 3. PROFİL TABLOLARI - Kullanıcı profilleri
    -- ================================================

    PRINT 'Profil tabloları soft delete yapılıyor...';

    -- AnimalMarket_VeterinarianProfiles tablosunda UserId ile ilişkili kayıtları işaretle
    IF OBJECT_ID('dbo.AnimalMarket_VeterinarianProfiles', 'U') IS NOT NULL
    BEGIN
        UPDATE AnimalMarket_VeterinarianProfiles
        SET IsDeleted = 1, DeletedAt = @deletedAt
        WHERE UserId = @userId AND IsDeleted = 0;
        PRINT '  - AnimalMarket_VeterinarianProfiles soft deleted';
    END
    ELSE
        PRINT '  - AnimalMarket_VeterinarianProfiles tablosu bulunamadı (atlanıyor)';

    -- AnimalMarket_TransporterProfiles tablosunda UserId ile ilişkili kayıtları işaretle
    IF OBJECT_ID('dbo.AnimalMarket_TransporterProfiles', 'U') IS NOT NULL
    BEGIN
        UPDATE AnimalMarket_TransporterProfiles
        SET IsDeleted = 1, DeletedAt = @deletedAt
        WHERE UserId = @userId AND IsDeleted = 0;
        PRINT '  - AnimalMarket_TransporterProfiles soft deleted';
    END
    ELSE
        PRINT '  - AnimalMarket_TransporterProfiles tablosu bulunamadı (atlanıyor)';

    -- AnimalMarket_AnimalOwnerProfiles tablosunda UserId ile ilişkili kayıtları işaretle
    IF OBJECT_ID('dbo.AnimalMarket_AnimalOwnerProfiles', 'U') IS NOT NULL
    BEGIN
        UPDATE AnimalMarket_AnimalOwnerProfiles
        SET IsDeleted = 1, DeletedAt = @deletedAt
        WHERE UserId = @userId AND IsDeleted = 0;
        PRINT '  - AnimalMarket_AnimalOwnerProfiles soft deleted';
    END
    ELSE
        PRINT '  - AnimalMarket_AnimalOwnerProfiles tablosu bulunamadı (atlanıyor)';

    -- ================================================
    -- 4. ANA TABLO - Kullanıcı tablosu
    -- ================================================

    PRINT 'Ana kullanıcı kaydı soft delete yapılıyor...';

    -- AppUsers tablosunda kullanıcıyı işaretle
    IF OBJECT_ID('dbo.AppUsers', 'U') IS NOT NULL
    BEGIN
        UPDATE AppUsers
        SET IsDeleted = 1, DeletedAt = @deletedAt, IsActive = 0
        WHERE Id = @userId AND IsDeleted = 0;
        PRINT '  - AppUsers soft deleted';
    END
    ELSE
    BEGIN
        PRINT '  - HATA: AppUsers tablosu bulunamadı!';
        THROW 50000, 'AppUsers tablosu bulunamadı. Veritabanı yapılandırmasını kontrol edin.', 1;
    END

    -- ================================================
    -- İŞLEM BAŞARILI - Değişiklikleri kaydet
    -- ================================================

    COMMIT TRANSACTION;

    PRINT '================================================';
    PRINT 'BAŞARILI: Kullanıcı ve tüm ilişkili veriler soft delete edildi!';
    PRINT 'User ID: ' + CAST(@userId AS NVARCHAR(50));
    PRINT 'Deleted At: ' + CAST(@deletedAt AS NVARCHAR(50));
    PRINT '================================================';

END TRY
BEGIN CATCH
    -- Hata durumunda işlemi geri al
    ROLLBACK TRANSACTION;

    PRINT '================================================';
    PRINT 'HATA: İşlem geri alındı!';
    PRINT 'Hata Mesajı: ' + ERROR_MESSAGE();
    PRINT 'Hata Satırı: ' + CAST(ERROR_LINE() AS NVARCHAR(10));
    PRINT '================================================';

    -- Hatayı tekrar fırlat
    THROW;
END CATCH;

GO
