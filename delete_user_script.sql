-- ================================================
-- KULLANICI SİLME SCRIPTI (HARD DELETE) - v4
-- ================================================

DECLARE @userId UNIQUEIDENTIFIER = '7F91CFD2';

BEGIN TRANSACTION;

BEGIN TRY
    PRINT '================================================';
    PRINT 'Kullanıcı silme işlemi başlıyor...';
    PRINT 'User ID: ' + CAST(@userId AS NVARCHAR(50));
    PRINT '================================================';

    -- ================================================
    -- 1. HIROVO MODÜLÜ
    -- ================================================
    PRINT 'Hirovo modülü temizleniyor...';

    IF OBJECT_ID('dbo.HirovoJobApplications', 'U') IS NOT NULL
        DELETE FROM HirovoJobApplications WHERE WorkerId = @userId;

    IF OBJECT_ID('dbo.HirovoJobs', 'U') IS NOT NULL
        DELETE FROM HirovoJobs WHERE HirovoEmployerId = @userId;

    IF OBJECT_ID('dbo.HirovoSubscriptions', 'U') IS NOT NULL
        DELETE FROM HirovoSubscriptions WHERE UserId = @userId;

    IF OBJECT_ID('dbo.HirovoFileAccessCodes', 'U') IS NOT NULL
        DELETE FROM HirovoFileAccessCodes WHERE GeneratorUserId = @userId;

    IF OBJECT_ID('dbo.HirovoDocumentRoots', 'U') IS NOT NULL
        DELETE FROM HirovoDocumentRoots WHERE CreatorUserId = @userId OR ManualCreatorUserId = @userId;

    -- ================================================
    -- 2. ANIMAL MARKET MODÜLÜ (Sıralama ÇOK ÖNEMLİ!)
    -- ================================================
    PRINT 'AnimalMarket modülü temizleniyor...';

    -- 2.1 Notifications
    IF OBJECT_ID('dbo.AnimalMarket_Notifications', 'U') IS NOT NULL
    BEGIN
        DELETE FROM AnimalMarket_Notifications WHERE UserId = @userId;
        PRINT '  - AnimalMarket_Notifications silindi';
    END

    -- 2.2 FarmUsers
    IF OBJECT_ID('dbo.AnimalMarket_FarmUsers', 'U') IS NOT NULL
    BEGIN
        DELETE FROM AnimalMarket_FarmUsers WHERE UserId = @userId;
        PRINT '  - AnimalMarket_FarmUsers silindi';
    END

    -- 2.3 VeterinaryApprovals - AnimalBids'dan ÖNCE silinmeli!
    IF OBJECT_ID('dbo.AnimalMarket_VeterinaryApprovals', 'U') IS NOT NULL
    BEGIN
        DELETE FROM AnimalMarket_VeterinaryApprovals WHERE VeterinarianId = @userId;
        PRINT '  - AnimalMarket_VeterinaryApprovals silindi (VeterinarianId)';
        
        IF OBJECT_ID('dbo.AnimalMarket_AnimalBids', 'U') IS NOT NULL
        BEGIN
            DELETE FROM AnimalMarket_VeterinaryApprovals 
            WHERE BidId IN (SELECT Id FROM AnimalMarket_AnimalBids WHERE BidderId = @userId);
            PRINT '  - AnimalMarket_VeterinaryApprovals silindi (BidId üzerinden)';
        END

        IF OBJECT_ID('dbo.AnimalMarket_Animals', 'U') IS NOT NULL
        BEGIN
            DELETE FROM AnimalMarket_VeterinaryApprovals 
            WHERE AnimalId IN (SELECT Id FROM AnimalMarket_Animals WHERE SellerId = @userId);
            PRINT '  - AnimalMarket_VeterinaryApprovals silindi (AnimalId üzerinden)';
        END
    END

    -- 2.4 DeliveryRequests
    IF OBJECT_ID('dbo.AnimalMarket_DeliveryRequests', 'U') IS NOT NULL
        AND OBJECT_ID('dbo.AnimalMarket_DeliveryOrders', 'U') IS NOT NULL
    BEGIN
        DELETE FROM AnimalMarket_DeliveryRequests 
        WHERE DeliveryOrderId IN (
            SELECT Id FROM AnimalMarket_DeliveryOrders 
            WHERE CarrierId = @userId OR BuyerId = @userId
        );
        PRINT '  - AnimalMarket_DeliveryRequests silindi (CarrierId/BuyerId)';

        IF OBJECT_ID('dbo.AnimalMarket_Animals', 'U') IS NOT NULL
        BEGIN
            DELETE FROM AnimalMarket_DeliveryRequests 
            WHERE DeliveryOrderId IN (
                SELECT Id FROM AnimalMarket_DeliveryOrders 
                WHERE AnimalId IN (SELECT Id FROM AnimalMarket_Animals WHERE SellerId = @userId)
            );
            PRINT '  - AnimalMarket_DeliveryRequests silindi (Animal üzerinden)';
        END
    END

    -- 2.5 DeliveryOrders
    IF OBJECT_ID('dbo.AnimalMarket_DeliveryOrders', 'U') IS NOT NULL
    BEGIN
        DELETE FROM AnimalMarket_DeliveryOrders WHERE CarrierId = @userId OR BuyerId = @userId;
        PRINT '  - AnimalMarket_DeliveryOrders silindi (CarrierId/BuyerId)';

        IF OBJECT_ID('dbo.AnimalMarket_Animals', 'U') IS NOT NULL
        BEGIN
            DELETE FROM AnimalMarket_DeliveryOrders 
            WHERE AnimalId IN (SELECT Id FROM AnimalMarket_Animals WHERE SellerId = @userId);
            PRINT '  - AnimalMarket_DeliveryOrders silindi (Animal üzerinden)';
        END
    END

    -- 2.6 AnimalBids
    IF OBJECT_ID('dbo.AnimalMarket_AnimalBids', 'U') IS NOT NULL
    BEGIN
        DELETE FROM AnimalMarket_AnimalBids WHERE BidderId = @userId;
        PRINT '  - AnimalMarket_AnimalBids silindi (BidderId)';

        IF OBJECT_ID('dbo.AnimalMarket_Animals', 'U') IS NOT NULL
        BEGIN
            DELETE FROM AnimalMarket_AnimalBids 
            WHERE AnimalId IN (SELECT Id FROM AnimalMarket_Animals WHERE SellerId = @userId);
            PRINT '  - AnimalMarket_AnimalBids silindi (Animal üzerinden)';
        END
    END

    -- 2.7 AnimalOffers
    IF OBJECT_ID('dbo.AnimalMarket_AnimalOffers', 'U') IS NOT NULL
    BEGIN
        DELETE FROM AnimalMarket_AnimalOffers WHERE BuyerId = @userId;
        PRINT '  - AnimalMarket_AnimalOffers silindi (BuyerId)';

        IF OBJECT_ID('dbo.AnimalMarket_Animals', 'U') IS NOT NULL
        BEGIN
            DELETE FROM AnimalMarket_AnimalOffers 
            WHERE AnimalId IN (SELECT Id FROM AnimalMarket_Animals WHERE SellerId = @userId);
            PRINT '  - AnimalMarket_AnimalOffers silindi (Animal üzerinden)';
        END
    END

    -- 2.8 EscrowTransactions
    IF OBJECT_ID('dbo.AnimalMarket_EscrowTransactions', 'U') IS NOT NULL
    BEGIN
        DELETE FROM AnimalMarket_EscrowTransactions WHERE BuyerId = @userId OR SellerId = @userId;
        PRINT '  - AnimalMarket_EscrowTransactions silindi';
    END

    -- 2.9 PaymentTransactions
    IF OBJECT_ID('dbo.AnimalMarket_PaymentTransactions', 'U') IS NOT NULL
    BEGIN
        DELETE FROM AnimalMarket_PaymentTransactions WHERE PayerId = @userId OR PayeeId = @userId;
        PRINT '  - AnimalMarket_PaymentTransactions silindi';
    END

    -- ════════════════════════════════════════════════════════════════
    -- 2.10 AnimalEarTags - Animals'dan ÖNCE silinmeli! (YENİ EKLENEN)
    -- ════════════════════════════════════════════════════════════════
    IF OBJECT_ID('dbo.AnimalMarket_AnimalEarTags', 'U') IS NOT NULL
        AND OBJECT_ID('dbo.AnimalMarket_Animals', 'U') IS NOT NULL
    BEGIN
        DELETE FROM AnimalMarket_AnimalEarTags 
        WHERE AnimalId IN (SELECT Id FROM AnimalMarket_Animals WHERE SellerId = @userId);
        PRINT '  - AnimalMarket_AnimalEarTags silindi';
    END

    -- ════════════════════════════════════════════════════════════════
    -- 2.11 AnimalImages - Animals'dan ÖNCE silinmeli! (YENİ EKLENEN)
    -- ════════════════════════════════════════════════════════════════
    IF OBJECT_ID('dbo.AnimalMarket_AnimalImages', 'U') IS NOT NULL
        AND OBJECT_ID('dbo.AnimalMarket_Animals', 'U') IS NOT NULL
    BEGIN
        DELETE FROM AnimalMarket_AnimalImages 
        WHERE AnimalId IN (SELECT Id FROM AnimalMarket_Animals WHERE SellerId = @userId);
        PRINT '  - AnimalMarket_AnimalImages silindi';
    END

    -- ════════════════════════════════════════════════════════════════
    -- 2.12 AnimalVideos - Animals'dan ÖNCE silinmeli! (YENİ EKLENEN)
    -- ════════════════════════════════════════════════════════════════
    IF OBJECT_ID('dbo.AnimalMarket_AnimalVideos', 'U') IS NOT NULL
        AND OBJECT_ID('dbo.AnimalMarket_Animals', 'U') IS NOT NULL
    BEGIN
        DELETE FROM AnimalMarket_AnimalVideos 
        WHERE AnimalId IN (SELECT Id FROM AnimalMarket_Animals WHERE SellerId = @userId);
        PRINT '  - AnimalMarket_AnimalVideos silindi';
    END

    -- ════════════════════════════════════════════════════════════════
    -- 2.13 AnimalDocuments - Animals'dan ÖNCE silinmeli! (YENİ EKLENEN)
    -- ════════════════════════════════════════════════════════════════
    IF OBJECT_ID('dbo.AnimalMarket_AnimalDocuments', 'U') IS NOT NULL
        AND OBJECT_ID('dbo.AnimalMarket_Animals', 'U') IS NOT NULL
    BEGIN
        DELETE FROM AnimalMarket_AnimalDocuments 
        WHERE AnimalId IN (SELECT Id FROM AnimalMarket_Animals WHERE SellerId = @userId);
        PRINT '  - AnimalMarket_AnimalDocuments silindi';
    END

    -- 2.14 Animals
    IF OBJECT_ID('dbo.AnimalMarket_Animals', 'U') IS NOT NULL
    BEGIN
        DELETE FROM AnimalMarket_Animals WHERE SellerId = @userId;
        PRINT '  - AnimalMarket_Animals silindi';
    END

    -- 2.15 Farms
    IF OBJECT_ID('dbo.AnimalMarket_Farms', 'U') IS NOT NULL
    BEGIN
        DELETE FROM AnimalMarket_Farms WHERE OwnerId = @userId;
        PRINT '  - AnimalMarket_Farms silindi';
    END

    -- ================================================
    -- 3. PROFİL TABLOLARI
    -- ================================================
    PRINT 'Profil tabloları temizleniyor...';

    IF OBJECT_ID('dbo.AnimalMarket_VeterinarianProfileServiceAreas', 'U') IS NOT NULL
        AND OBJECT_ID('dbo.AnimalMarket_VeterinarianProfiles', 'U') IS NOT NULL
    BEGIN
        DELETE FROM AnimalMarket_VeterinarianProfileServiceAreas
        WHERE ProfileId IN (SELECT Id FROM AnimalMarket_VeterinarianProfiles WHERE UserId = @userId);
        PRINT '  - AnimalMarket_VeterinarianProfileServiceAreas silindi';
    END

    IF OBJECT_ID('dbo.AnimalMarket_VeterinarianProfileCertifications', 'U') IS NOT NULL
        AND OBJECT_ID('dbo.AnimalMarket_VeterinarianProfiles', 'U') IS NOT NULL
    BEGIN
        DELETE FROM AnimalMarket_VeterinarianProfileCertifications
        WHERE ProfileId IN (SELECT Id FROM AnimalMarket_VeterinarianProfiles WHERE UserId = @userId);
        PRINT '  - AnimalMarket_VeterinarianProfileCertifications silindi';
    END

    IF OBJECT_ID('dbo.AnimalMarket_VeterinarianProfiles', 'U') IS NOT NULL
    BEGIN
        DELETE FROM AnimalMarket_VeterinarianProfiles WHERE UserId = @userId;
        PRINT '  - AnimalMarket_VeterinarianProfiles silindi';
    END

    IF OBJECT_ID('dbo.AnimalMarket_TransporterVehicleCapacities', 'U') IS NOT NULL
        AND OBJECT_ID('dbo.AnimalMarket_TransporterProfiles', 'U') IS NOT NULL
    BEGIN
        DELETE FROM AnimalMarket_TransporterVehicleCapacities
        WHERE TransporterProfileId IN (SELECT Id FROM AnimalMarket_TransporterProfiles WHERE UserId = @userId);
        PRINT '  - AnimalMarket_TransporterVehicleCapacities silindi';
    END

    IF OBJECT_ID('dbo.AnimalMarket_TransporterProfiles', 'U') IS NOT NULL
    BEGIN
        DELETE FROM AnimalMarket_TransporterProfiles WHERE UserId = @userId;
        PRINT '  - AnimalMarket_TransporterProfiles silindi';
    END

    IF OBJECT_ID('dbo.AnimalMarket_AnimalOwnerProfiles', 'U') IS NOT NULL
    BEGIN
        DELETE FROM AnimalMarket_AnimalOwnerProfiles WHERE UserId = @userId;
        PRINT '  - AnimalMarket_AnimalOwnerProfiles silindi';
    END

    -- ================================================
    -- 4. KULLANICI SÖZLEŞME KABULLERI
    -- ================================================
    IF OBJECT_ID('dbo.AnimalMarket_UserAgreementAcceptances', 'U') IS NOT NULL
    BEGIN
        DELETE FROM AnimalMarket_UserAgreementAcceptances WHERE UserId = @userId;
        PRINT '  - AnimalMarket_UserAgreementAcceptances silindi';
    END

    -- ================================================
    -- 5. PUSH TOKEN'LAR
    -- ================================================
    IF OBJECT_ID('dbo.AppUserPushTokens', 'U') IS NOT NULL
    BEGIN
        DELETE FROM AppUserPushTokens WHERE UserId = @userId;
        PRINT '  - AppUserPushTokens silindi';
    END

    -- ================================================
    -- 6. ORTAK TABLOLAR
    -- ================================================
    PRINT 'Ortak tablolar temizleniyor...';

    IF OBJECT_ID('dbo.AppAuditLogs', 'U') IS NOT NULL
    BEGIN
        IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('AppAuditLogs') AND name = 'UserId')
            DELETE FROM AppAuditLogs WHERE UserId = @userId;
        PRINT '  - AppAuditLogs silindi';
    END

    IF OBJECT_ID('dbo.UserRoles', 'U') IS NOT NULL
    BEGIN
        DELETE FROM UserRoles WHERE UserId = @userId;
        PRINT '  - UserRoles silindi';
    END

    -- ================================================
    -- 7. ANA TABLO - AppUsers
    -- ================================================
    PRINT 'Ana kullanıcı kaydı siliniyor...';

    IF OBJECT_ID('dbo.AppUsers', 'U') IS NOT NULL
    BEGIN
        DELETE FROM AppUsers WHERE Id = @userId;
        PRINT '  - AppUsers silindi';
    END

    COMMIT TRANSACTION;

    PRINT '================================================';
    PRINT 'BAŞARILI: Kullanıcı ve tüm ilişkili veriler silindi!';
    PRINT 'User ID: ' + CAST(@userId AS NVARCHAR(50));
    PRINT '================================================';

END TRY
BEGIN CATCH
    ROLLBACK TRANSACTION;

    PRINT '================================================';
    PRINT 'HATA: İşlem geri alındı!';
    PRINT 'Hata Mesajı: ' + ERROR_MESSAGE();
    PRINT 'Hata Satırı: ' + CAST(ERROR_LINE() AS NVARCHAR(10));
    PRINT '================================================';

    THROW;
END CATCH;

GO