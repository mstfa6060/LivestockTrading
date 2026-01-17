-- ================================================
-- KULLANICI SİLME SCRIPTI (HARD DELETE) - v3.2
-- ================================================

DECLARE @userId UNIQUEIDENTIFIER = 'BURAYA_SİLİNECEK_KULLANICI_ID_GİRİNİZ';
DECLARE @deletedCount INT;
DECLARE @animalCount INT;

BEGIN TRANSACTION;

BEGIN TRY
    PRINT '================================================';
    PRINT 'Kullanıcı silme işlemi başlıyor...';
    PRINT 'User ID: ' + CAST(@userId AS NVARCHAR(50));
    PRINT '================================================';

    -- 0. KULLANICININ HAYVANLARINI BUL
    CREATE TABLE #UserAnimals
(
    AnimalId UNIQUEIDENTIFIER
);
    IF OBJECT_ID('dbo.AnimalMarket_Animals', 'U') IS NOT NULL
    BEGIN
    INSERT INTO #UserAnimals
        (AnimalId)
    SELECT Id
    FROM AnimalMarket_Animals
    WHERE SellerId = @userId;
    SET @animalCount = @@ROWCOUNT;
    PRINT 'Kullanıcının ' + CAST(@animalCount AS NVARCHAR(10)) + ' adet hayvanı bulundu.';
END

    -- 1. HIROVO
    PRINT ''; PRINT 'Hirovo modülü temizleniyor...';
    IF OBJECT_ID('dbo.HirovoJobApplications', 'U') IS NOT NULL BEGIN
    DELETE FROM HirovoJobApplications WHERE WorkerId = @userId;
    SET @deletedCount = @@ROWCOUNT;
    PRINT '  - HirovoJobApplications: ' + CAST(@deletedCount AS NVARCHAR(10));
END
    IF OBJECT_ID('dbo.HirovoJobs', 'U') IS NOT NULL BEGIN
    DELETE FROM HirovoJobs WHERE HirovoEmployerId = @userId;
    SET @deletedCount = @@ROWCOUNT;
    PRINT '  - HirovoJobs: ' + CAST(@deletedCount AS NVARCHAR(10));
END
    IF OBJECT_ID('dbo.HirovoSubscriptions', 'U') IS NOT NULL BEGIN
    DELETE FROM HirovoSubscriptions WHERE UserId = @userId;
    SET @deletedCount = @@ROWCOUNT;
    PRINT '  - HirovoSubscriptions: ' + CAST(@deletedCount AS NVARCHAR(10));
END
    IF OBJECT_ID('dbo.HirovoFileAccessCodes', 'U') IS NOT NULL BEGIN
    DELETE FROM HirovoFileAccessCodes WHERE GeneratorUserId = @userId;
    SET @deletedCount = @@ROWCOUNT;
    PRINT '  - HirovoFileAccessCodes: ' + CAST(@deletedCount AS NVARCHAR(10));
END
    IF OBJECT_ID('dbo.HirovoDocumentRoots', 'U') IS NOT NULL BEGIN
    DELETE FROM HirovoDocumentRoots WHERE CreatorUserId = @userId OR ManualCreatorUserId = @userId;
    SET @deletedCount = @@ROWCOUNT;
    PRINT '  - HirovoDocumentRoots: ' + CAST(@deletedCount AS NVARCHAR(10));
END

    -- 2. ANIMAL MARKET - HAYVAN BAĞIMLILIKLARI
    PRINT ''; PRINT 'AnimalMarket - Hayvan bağımlılıkları temizleniyor...';
    IF OBJECT_ID('dbo.AnimalMarket_AnimalBids', 'U') IS NOT NULL BEGIN
    DELETE FROM AnimalMarket_AnimalBids WHERE AnimalId IN (SELECT AnimalId
    FROM #UserAnimals);
    SET @deletedCount = @@ROWCOUNT;
    PRINT '  - AnimalBids (hayvan): ' + CAST(@deletedCount AS NVARCHAR(10));
END
    IF OBJECT_ID('dbo.AnimalMarket_AnimalOffers', 'U') IS NOT NULL BEGIN
    DELETE FROM AnimalMarket_AnimalOffers WHERE AnimalId IN (SELECT AnimalId
    FROM #UserAnimals);
    SET @deletedCount = @@ROWCOUNT;
    PRINT '  - AnimalOffers (hayvan): ' + CAST(@deletedCount AS NVARCHAR(10));
END
    IF OBJECT_ID('dbo.AnimalMarket_VeterinaryDocuments', 'U') IS NOT NULL BEGIN
    DELETE FROM AnimalMarket_VeterinaryDocuments WHERE AnimalId IN (SELECT AnimalId
    FROM #UserAnimals);
    SET @deletedCount = @@ROWCOUNT;
    PRINT '  - VeterinaryDocuments: ' + CAST(@deletedCount AS NVARCHAR(10));
END
    IF OBJECT_ID('dbo.AnimalMarket_VeterinaryApprovals', 'U') IS NOT NULL BEGIN
    DELETE FROM AnimalMarket_VeterinaryApprovals WHERE AnimalId IN (SELECT AnimalId
    FROM #UserAnimals);
    SET @deletedCount = @@ROWCOUNT;
    PRINT '  - VeterinaryApprovals (hayvan): ' + CAST(@deletedCount AS NVARCHAR(10));
END
    IF OBJECT_ID('dbo.AnimalMarket_VetAssignments', 'U') IS NOT NULL BEGIN
    DELETE FROM AnimalMarket_VetAssignments WHERE AnimalId IN (SELECT AnimalId
    FROM #UserAnimals);
    SET @deletedCount = @@ROWCOUNT;
    PRINT '  - VetAssignments: ' + CAST(@deletedCount AS NVARCHAR(10));
END
    IF OBJECT_ID('dbo.AnimalMarket_EscrowTransactions', 'U') IS NOT NULL BEGIN
    DELETE FROM AnimalMarket_EscrowTransactions WHERE AnimalId IN (SELECT AnimalId
    FROM #UserAnimals);
    SET @deletedCount = @@ROWCOUNT;
    PRINT '  - EscrowTransactions (hayvan): ' + CAST(@deletedCount AS NVARCHAR(10));
END
    IF OBJECT_ID('dbo.AnimalMarket_PaymentTransactions', 'U') IS NOT NULL BEGIN
    DELETE FROM AnimalMarket_PaymentTransactions WHERE AnimalId IN (SELECT AnimalId
    FROM #UserAnimals);
    SET @deletedCount = @@ROWCOUNT;
    PRINT '  - PaymentTransactions (hayvan): ' + CAST(@deletedCount AS NVARCHAR(10));
END
    IF OBJECT_ID('dbo.AnimalMarket_DeliveryRequests', 'U') IS NOT NULL AND OBJECT_ID('dbo.AnimalMarket_DeliveryOrders', 'U') IS NOT NULL BEGIN
    DELETE FROM AnimalMarket_DeliveryRequests WHERE DeliveryOrderId IN (SELECT Id
    FROM AnimalMarket_DeliveryOrders
    WHERE AnimalId IN (SELECT AnimalId
    FROM #UserAnimals));
    SET @deletedCount = @@ROWCOUNT;
    PRINT '  - DeliveryRequests (hayvan): ' + CAST(@deletedCount AS NVARCHAR(10));
END
    IF OBJECT_ID('dbo.AnimalMarket_DeliveryOrders', 'U') IS NOT NULL BEGIN
    DELETE FROM AnimalMarket_DeliveryOrders WHERE AnimalId IN (SELECT AnimalId
    FROM #UserAnimals);
    SET @deletedCount = @@ROWCOUNT;
    PRINT '  - DeliveryOrders (hayvan): ' + CAST(@deletedCount AS NVARCHAR(10));
END

    -- 3. ANIMAL MARKET - KULLANICI İŞLEMLERİ
    PRINT ''; PRINT 'AnimalMarket - Kullanıcı işlemleri temizleniyor...';
    IF OBJECT_ID('dbo.AnimalMarket_AnimalBids', 'U') IS NOT NULL BEGIN
    DELETE FROM AnimalMarket_AnimalBids WHERE BidderId = @userId;
    SET @deletedCount = @@ROWCOUNT;
    PRINT '  - AnimalBids (kullanıcı): ' + CAST(@deletedCount AS NVARCHAR(10));
END
    IF OBJECT_ID('dbo.AnimalMarket_AnimalOffers', 'U') IS NOT NULL BEGIN
    DELETE FROM AnimalMarket_AnimalOffers WHERE BuyerId = @userId;
    SET @deletedCount = @@ROWCOUNT;
    PRINT '  - AnimalOffers (kullanıcı): ' + CAST(@deletedCount AS NVARCHAR(10));
END
    IF OBJECT_ID('dbo.AnimalMarket_VeterinaryApprovals', 'U') IS NOT NULL BEGIN
    DELETE FROM AnimalMarket_VeterinaryApprovals WHERE VeterinarianId = @userId;
    SET @deletedCount = @@ROWCOUNT;
    PRINT '  - VeterinaryApprovals (vet): ' + CAST(@deletedCount AS NVARCHAR(10));
END
    IF OBJECT_ID('dbo.AnimalMarket_EscrowTransactions', 'U') IS NOT NULL BEGIN
    DELETE FROM AnimalMarket_EscrowTransactions WHERE BuyerId = @userId OR SellerId = @userId;
    SET @deletedCount = @@ROWCOUNT;
    PRINT '  - EscrowTransactions (kullanıcı): ' + CAST(@deletedCount AS NVARCHAR(10));
END
    IF OBJECT_ID('dbo.AnimalMarket_PaymentTransactions', 'U') IS NOT NULL BEGIN
    DELETE FROM AnimalMarket_PaymentTransactions WHERE PayerId = @userId OR PayeeId = @userId;
    SET @deletedCount = @@ROWCOUNT;
    PRINT '  - PaymentTransactions (kullanıcı): ' + CAST(@deletedCount AS NVARCHAR(10));
END
    IF OBJECT_ID('dbo.AnimalMarket_DeliveryRequests', 'U') IS NOT NULL AND OBJECT_ID('dbo.AnimalMarket_TransporterProfiles', 'U') IS NOT NULL BEGIN
    DELETE FROM AnimalMarket_DeliveryRequests WHERE TransporterProfileId IN (SELECT Id
    FROM AnimalMarket_TransporterProfiles
    WHERE UserId = @userId);
    SET @deletedCount = @@ROWCOUNT;
    PRINT '  - DeliveryRequests (nakliyeci): ' + CAST(@deletedCount AS NVARCHAR(10));
END
    IF OBJECT_ID('dbo.AnimalMarket_DeliveryOrders', 'U') IS NOT NULL BEGIN
    DELETE FROM AnimalMarket_DeliveryOrders WHERE CarrierId = @userId OR BuyerId = @userId;
    SET @deletedCount = @@ROWCOUNT;
    PRINT '  - DeliveryOrders (kullanıcı): ' + CAST(@deletedCount AS NVARCHAR(10));
END
    IF OBJECT_ID('dbo.AnimalMarket_FarmUsers', 'U') IS NOT NULL BEGIN
    DELETE FROM AnimalMarket_FarmUsers WHERE UserId = @userId;
    SET @deletedCount = @@ROWCOUNT;
    PRINT '  - FarmUsers: ' + CAST(@deletedCount AS NVARCHAR(10));
END

    -- 4. ANA TABLOLAR
    PRINT ''; PRINT 'Ana tablolar temizleniyor...';
    IF OBJECT_ID('dbo.AnimalMarket_Animals', 'U') IS NOT NULL BEGIN
    DELETE FROM AnimalMarket_Animals WHERE SellerId = @userId;
    SET @deletedCount = @@ROWCOUNT;
    PRINT '  - Animals: ' + CAST(@deletedCount AS NVARCHAR(10));
END
    IF OBJECT_ID('dbo.AnimalMarket_FarmUsers', 'U') IS NOT NULL AND OBJECT_ID('dbo.AnimalMarket_Farms', 'U') IS NOT NULL BEGIN
    DELETE FROM AnimalMarket_FarmUsers WHERE FarmId IN (SELECT Id
    FROM AnimalMarket_Farms
    WHERE OwnerId = @userId);
    SET @deletedCount = @@ROWCOUNT;
    PRINT '  - FarmUsers (çiftlik): ' + CAST(@deletedCount AS NVARCHAR(10));
END
    IF OBJECT_ID('dbo.AnimalMarket_Farms', 'U') IS NOT NULL BEGIN
    DELETE FROM AnimalMarket_Farms WHERE OwnerId = @userId;
    SET @deletedCount = @@ROWCOUNT;
    PRINT '  - Farms: ' + CAST(@deletedCount AS NVARCHAR(10));
END

    -- 5. PROFİLLER
    PRINT ''; PRINT 'Profiller temizleniyor...';
    IF OBJECT_ID('dbo.AnimalMarket_VeterinarianProfileServiceAreas', 'U') IS NOT NULL AND OBJECT_ID('dbo.AnimalMarket_VeterinarianProfiles', 'U') IS NOT NULL BEGIN
    DELETE FROM AnimalMarket_VeterinarianProfileServiceAreas WHERE ProfileId IN (SELECT Id
    FROM AnimalMarket_VeterinarianProfiles
    WHERE UserId = @userId);
    SET @deletedCount = @@ROWCOUNT;
    PRINT '  - VetProfileServiceAreas: ' + CAST(@deletedCount AS NVARCHAR(10));
END
    IF OBJECT_ID('dbo.AnimalMarket_VeterinarianProfileCertifications', 'U') IS NOT NULL AND OBJECT_ID('dbo.AnimalMarket_VeterinarianProfiles', 'U') IS NOT NULL BEGIN
    DELETE FROM AnimalMarket_VeterinarianProfileCertifications WHERE ProfileId IN (SELECT Id
    FROM AnimalMarket_VeterinarianProfiles
    WHERE UserId = @userId);
    SET @deletedCount = @@ROWCOUNT;
    PRINT '  - VetProfileCerts: ' + CAST(@deletedCount AS NVARCHAR(10));
END
    IF OBJECT_ID('dbo.AnimalMarket_VeterinarianProfiles', 'U') IS NOT NULL BEGIN
    DELETE FROM AnimalMarket_VeterinarianProfiles WHERE UserId = @userId;
    SET @deletedCount = @@ROWCOUNT;
    PRINT '  - VeterinarianProfiles: ' + CAST(@deletedCount AS NVARCHAR(10));
END
    IF OBJECT_ID('dbo.AnimalMarket_TransporterVehicleCapacities', 'U') IS NOT NULL AND OBJECT_ID('dbo.AnimalMarket_TransporterProfiles', 'U') IS NOT NULL BEGIN
    DELETE FROM AnimalMarket_TransporterVehicleCapacities WHERE TransporterProfileId IN (SELECT Id
    FROM AnimalMarket_TransporterProfiles
    WHERE UserId = @userId);
    SET @deletedCount = @@ROWCOUNT;
    PRINT '  - TransporterCapacities: ' + CAST(@deletedCount AS NVARCHAR(10));
END
    IF OBJECT_ID('dbo.AnimalMarket_TransporterProfiles', 'U') IS NOT NULL BEGIN
    DELETE FROM AnimalMarket_TransporterProfiles WHERE UserId = @userId;
    SET @deletedCount = @@ROWCOUNT;
    PRINT '  - TransporterProfiles: ' + CAST(@deletedCount AS NVARCHAR(10));
END
    IF OBJECT_ID('dbo.AnimalMarket_AnimalOwnerProfiles', 'U') IS NOT NULL BEGIN
    DELETE FROM AnimalMarket_AnimalOwnerProfiles WHERE UserId = @userId;
    SET @deletedCount = @@ROWCOUNT;
    PRINT '  - AnimalOwnerProfiles: ' + CAST(@deletedCount AS NVARCHAR(10));
END
    IF OBJECT_ID('dbo.AnimalMarket_SellerPaymentInfos', 'U') IS NOT NULL BEGIN
    DELETE FROM AnimalMarket_SellerPaymentInfos WHERE UserId = @userId;
    SET @deletedCount = @@ROWCOUNT;
    PRINT '  - SellerPaymentInfos: ' + CAST(@deletedCount AS NVARCHAR(10));
END

    -- 6. ORTAK TABLOLAR
    PRINT ''; PRINT 'Ortak tablolar temizleniyor...';
    IF OBJECT_ID('dbo.AnimalMarket_Notifications', 'U') IS NOT NULL BEGIN
    DELETE FROM AnimalMarket_Notifications WHERE UserId = @userId;
    SET @deletedCount = @@ROWCOUNT;
    PRINT '  - AnimalMarket_Notifications: ' + CAST(@deletedCount AS NVARCHAR(10));
END
    IF OBJECT_ID('dbo.AppUserLocations', 'U') IS NOT NULL BEGIN
    DELETE FROM AppUserLocations WHERE UserId = @userId;
    SET @deletedCount = @@ROWCOUNT;
    PRINT '  - AppUserLocations: ' + CAST(@deletedCount AS NVARCHAR(10));
END
    IF OBJECT_ID('dbo.AppUserPushTokens', 'U') IS NOT NULL BEGIN
    DELETE FROM AppUserPushTokens WHERE UserId = @userId;
    SET @deletedCount = @@ROWCOUNT;
    PRINT '  - AppUserPushTokens: ' + CAST(@deletedCount AS NVARCHAR(10));
END
    IF OBJECT_ID('dbo.UserRoles', 'U') IS NOT NULL BEGIN
    DELETE FROM UserRoles WHERE UserId = @userId;
    SET @deletedCount = @@ROWCOUNT;
    PRINT '  - UserRoles: ' + CAST(@deletedCount AS NVARCHAR(10));
END
    IF OBJECT_ID('dbo.AppRefreshTokens', 'U') IS NOT NULL BEGIN
    DELETE FROM AppRefreshTokens WHERE UserId = @userId;
    SET @deletedCount = @@ROWCOUNT;
    PRINT '  - AppRefreshTokens: ' + CAST(@deletedCount AS NVARCHAR(10));
END

    -- 7. KULLANICI
    PRINT ''; PRINT 'Kullanıcı siliniyor...';
    IF OBJECT_ID('dbo.AppUsers', 'U') IS NOT NULL BEGIN
    DELETE FROM AppUsers WHERE Id = @userId;
    SET @deletedCount = @@ROWCOUNT;
    PRINT '  - AppUsers: ' + CAST(@deletedCount AS NVARCHAR(10));
END

    DROP TABLE #UserAnimals;
    COMMIT TRANSACTION;
    PRINT ''; PRINT '================================================';
    PRINT 'BAŞARILI: Kullanıcı silindi!';
    PRINT '================================================';

END TRY
BEGIN CATCH
    ROLLBACK TRANSACTION;
    IF OBJECT_ID('tempdb..#UserAnimals') IS NOT NULL DROP TABLE #UserAnimals;
    PRINT ''; PRINT '================================================';
    PRINT 'HATA: ' + ERROR_MESSAGE();
    PRINT '================================================';
    THROW;
END CATCH;
GO
