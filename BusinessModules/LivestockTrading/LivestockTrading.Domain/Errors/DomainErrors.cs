namespace LivestockTrading.Domain.Errors;

public class LivestockTradingDomainErrors
{
    public static class CommonErrors
    {
        public static string IdNotValid { get; set; } = "ID gecersiz.";
        public static string NameNotValid { get; set; } = "Isim gecersiz.";
        public static string PageNotValid { get; set; } = "Sayfa numarasi gecersiz.";
        public static string PageSizeNotValid { get; set; } = "Sayfa boyutu gecersiz.";
        public static string UnauthorizedAccess { get; set; } = "Yetkisiz erisim.";
        public static string InvalidEnum { get; set; } = "Enum degeri gecersiz.";
        public static string ActorRequired { get; set; } = "Aktor bilgisi gereklidir.";
        public static string InvalidNumber { get; set; } = "Sayisal deger gecersiz.";
        public static string ValueTooLong { get; set; } = "Deger cok uzun.";
    }

    public static class CategoryErrors
    {
        public static string CategoryNotFound { get; set; } = "Kategori bulunamadi.";
        public static string CategoryNotActive { get; set; } = "Kategori aktif degil.";
        public static string ParentCategoryNotFound { get; set; } = "Ust kategori bulunamadi.";
        public static string CategorySlugAlreadyExists { get; set; } = "Bu slug zaten kullaniliyor.";
        public static string CategoryNameAlreadyExists { get; set; } = "Bu isimde bir kategori zaten mevcut.";
        public static string CategoryHasChildren { get; set; } = "Alt kategorileri olan bir kategori silinemez.";
        public static string CategoryNameRequired { get; set; } = "Kategori adi zorunludur.";
        public static string CategorySlugRequired { get; set; } = "Kategori slug zorunludur.";
    }

    public static class BrandErrors
    {
        public static string BrandNotFound { get; set; } = "Marka bulunamadi.";
        public static string BrandNotActive { get; set; } = "Marka aktif degil.";
        public static string BrandSlugAlreadyExists { get; set; } = "Bu slug zaten kullaniliyor.";
        public static string BrandNameRequired { get; set; } = "Marka adi zorunludur.";
        public static string BrandSlugRequired { get; set; } = "Marka slug zorunludur.";
    }

    public static class ProductErrors
    {
        public static string ProductNotFound { get; set; } = "Urun bulunamadi.";
        public static string ProductNotActive { get; set; } = "Urun aktif degil.";
        public static string ProductSlugAlreadyExists { get; set; } = "Bu slug zaten kullaniliyor.";
        public static string ProductTitleRequired { get; set; } = "Urun basligi zorunludur.";
        public static string ProductSlugRequired { get; set; } = "Urun slug zorunludur.";
        public static string ProductCategoryRequired { get; set; } = "Kategori zorunludur.";
        public static string ProductSellerRequired { get; set; } = "Satici zorunludur.";
        public static string ProductLocationRequired { get; set; } = "Konum zorunludur.";
        public static string ProductPriceRequired { get; set; } = "Fiyat zorunludur.";
        public static string ProductNotPendingApproval { get; set; } = "Urun onay bekleyen durumda degil.";
        public static string ProductAlreadyApproved { get; set; } = "Urun zaten onaylanmis.";
        public static string ProductAlreadyRejected { get; set; } = "Urun zaten reddedilmis.";
        public static string RejectionReasonRequired { get; set; } = "Red nedeni zorunludur.";
    }

    public static class LocationErrors
    {
        public static string LocationNotFound { get; set; } = "Konum bulunamadi.";
        public static string LocationNameRequired { get; set; } = "Konum adi zorunludur.";
    }

    public static class SellerErrors
    {
        public static string SellerNotFound { get; set; } = "Satici bulunamadi.";
        public static string SellerBusinessNameRequired { get; set; } = "Isletme adi zorunludur.";
        public static string SellerUserIdRequired { get; set; } = "Kullanici ID zorunludur.";
        public static string SellerNotPendingVerification { get; set; } = "Satici dogrulama bekleyen durumda degil.";
        public static string SellerAlreadySuspended { get; set; } = "Satici zaten askiya alinmis.";
        public static string SuspensionReasonRequired { get; set; } = "Askiya alma nedeni zorunludur.";
    }

    public static class FarmErrors
    {
        public static string FarmNotFound { get; set; } = "Ciftlik bulunamadi.";
        public static string FarmNameRequired { get; set; } = "Ciftlik adi zorunludur.";
        public static string FarmSellerRequired { get; set; } = "Satici zorunludur.";
        public static string FarmLocationRequired { get; set; } = "Konum zorunludur.";
    }

    public static class ProductVariantErrors
    {
        public static string VariantNotFound { get; set; } = "Urun varyanti bulunamadi.";
        public static string VariantNameRequired { get; set; } = "Varyant adi zorunludur.";
        public static string VariantProductRequired { get; set; } = "Urun zorunludur.";
    }

    public static class ProductPriceErrors
    {
        public static string PriceNotFound { get; set; } = "Fiyat bilgisi bulunamadi.";
        public static string PriceCurrencyCodeRequired { get; set; } = "Para birimi kodu zorunludur.";
        public static string PriceAmountRequired { get; set; } = "Fiyat zorunludur.";
        public static string PriceProductRequired { get; set; } = "Urun zorunludur.";
    }

    public static class ProductImageErrors
    {
        public static string ImageNotFound { get; set; } = "Gorsel bulunamadi.";
        public static string ImageUrlRequired { get; set; } = "Gorsel URL zorunludur.";
        public static string ImageProductRequired { get; set; } = "Urun zorunludur.";
    }

    public static class ProductVideoErrors
    {
        public static string VideoNotFound { get; set; } = "Video bulunamadi.";
        public static string VideoUrlRequired { get; set; } = "Video URL zorunludur.";
        public static string VideoProductRequired { get; set; } = "Urun zorunludur.";
    }

    public static class ProductDocumentErrors
    {
        public static string DocumentNotFound { get; set; } = "Dokuman bulunamadi.";
        public static string DocumentUrlRequired { get; set; } = "Dokuman URL zorunludur.";
        public static string DocumentFileNameRequired { get; set; } = "Dosya adi zorunludur.";
        public static string DocumentProductRequired { get; set; } = "Urun zorunludur.";
    }

    public static class AnimalInfoErrors
    {
        public static string AnimalInfoNotFound { get; set; } = "Hayvan bilgisi bulunamadi.";
        public static string AnimalBreedNameRequired { get; set; } = "Irk adi zorunludur.";
        public static string AnimalProductRequired { get; set; } = "Urun zorunludur.";
    }

    public static class HealthRecordErrors
    {
        public static string HealthRecordNotFound { get; set; } = "Saglik kaydi bulunamadi.";
        public static string HealthRecordTypeRequired { get; set; } = "Kayit tipi zorunludur.";
        public static string HealthRecordDateRequired { get; set; } = "Kayit tarihi zorunludur.";
        public static string HealthRecordAnimalInfoRequired { get; set; } = "Hayvan bilgisi zorunludur.";
    }

    public static class VaccinationErrors
    {
        public static string VaccinationNotFound { get; set; } = "Asilanma kaydi bulunamadi.";
        public static string VaccinationNameRequired { get; set; } = "Asi adi zorunludur.";
        public static string VaccinationDateRequired { get; set; } = "Asilanma tarihi zorunludur.";
        public static string VaccinationAnimalInfoRequired { get; set; } = "Hayvan bilgisi zorunludur.";
    }

    public static class ChemicalInfoErrors
    {
        public static string ChemicalInfoNotFound { get; set; } = "Kimyasal bilgisi bulunamadi.";
        public static string ChemicalProductRequired { get; set; } = "Urun zorunludur.";
    }

    public static class MachineryInfoErrors
    {
        public static string MachineryInfoNotFound { get; set; } = "Makine bilgisi bulunamadi.";
        public static string MachineryProductRequired { get; set; } = "Urun zorunludur.";
    }

    public static class SeedInfoErrors
    {
        public static string SeedInfoNotFound { get; set; } = "Tohum bilgisi bulunamadi.";
        public static string SeedProductRequired { get; set; } = "Urun zorunludur.";
    }

    public static class FeedInfoErrors
    {
        public static string FeedInfoNotFound { get; set; } = "Yem bilgisi bulunamadi.";
        public static string FeedProductRequired { get; set; } = "Urun zorunludur.";
    }

    public static class VeterinaryInfoErrors
    {
        public static string VeterinaryInfoNotFound { get; set; } = "Veteriner urun bilgisi bulunamadi.";
        public static string VeterinaryProductRequired { get; set; } = "Urun zorunludur.";
    }

    public static class ProductReviewErrors
    {
        public static string ProductReviewNotFound { get; set; } = "Degerlendirme bulunamadi.";
        public static string ProductReviewProductRequired { get; set; } = "Urun zorunludur.";
        public static string ProductReviewUserRequired { get; set; } = "Kullanici zorunludur.";
        public static string ProductReviewRatingRequired { get; set; } = "Puan zorunludur.";
    }

    public static class SellerReviewErrors
    {
        public static string SellerReviewNotFound { get; set; } = "Satici degerlendirmesi bulunamadi.";
        public static string SellerReviewSellerRequired { get; set; } = "Satici zorunludur.";
        public static string SellerReviewUserRequired { get; set; } = "Kullanici zorunludur.";
        public static string SellerReviewRatingRequired { get; set; } = "Puan zorunludur.";
    }

    public static class TransporterReviewErrors
    {
        public static string TransporterReviewNotFound { get; set; } = "Tasiyici degerlendirmesi bulunamadi.";
        public static string TransporterReviewTransporterRequired { get; set; } = "Tasiyici zorunludur.";
        public static string TransporterReviewUserRequired { get; set; } = "Kullanici zorunludur.";
        public static string TransporterReviewRatingRequired { get; set; } = "Puan zorunludur.";
    }

    public static class FavoriteProductErrors
    {
        public static string FavoriteNotFound { get; set; } = "Favori bulunamadi.";
        public static string FavoriteProductRequired { get; set; } = "Urun zorunludur.";
        public static string FavoriteUserRequired { get; set; } = "Kullanici zorunludur.";
    }

    public static class NotificationErrors
    {
        public static string NotificationNotFound { get; set; } = "Bildirim bulunamadi.";
        public static string NotificationTitleRequired { get; set; } = "Baslik zorunludur.";
        public static string NotificationMessageRequired { get; set; } = "Mesaj zorunludur.";
        public static string NotificationUserRequired { get; set; } = "Kullanici zorunludur.";
    }

    public static class SearchHistoryErrors
    {
        public static string SearchHistoryNotFound { get; set; } = "Arama gecmisi bulunamadi.";
        public static string SearchHistoryQueryRequired { get; set; } = "Arama sorgusu zorunludur.";
        public static string SearchHistoryUserRequired { get; set; } = "Kullanici zorunludur.";
    }

    public static class ProductViewHistoryErrors
    {
        public static string ViewHistoryNotFound { get; set; } = "Goruntuleme gecmisi bulunamadi.";
        public static string ViewHistoryProductRequired { get; set; } = "Urun zorunludur.";
        public static string ViewHistoryUserRequired { get; set; } = "Kullanici zorunludur.";
    }

    public static class UserPreferencesErrors
    {
        public static string PreferencesNotFound { get; set; } = "Kullanici tercihleri bulunamadi.";
        public static string PreferencesUserRequired { get; set; } = "Kullanici zorunludur.";
    }

    public static class ConversationErrors
    {
        public static string ConversationNotFound { get; set; } = "Konusma bulunamadi.";
        public static string ConversationParticipantsRequired { get; set; } = "Katilimcilar zorunludur.";
    }

    public static class MessageErrors
    {
        public static string MessageNotFound { get; set; } = "Mesaj bulunamadi.";
        public static string MessageContentRequired { get; set; } = "Icerik zorunludur.";
        public static string MessageConversationRequired { get; set; } = "Konusma zorunludur.";
        public static string MessageSenderRequired { get; set; } = "Gonderen zorunludur.";
        public static string MessageRecipientRequired { get; set; } = "Alici zorunludur.";
    }

    public static class OfferErrors
    {
        public static string OfferNotFound { get; set; } = "Teklif bulunamadi.";
        public static string OfferProductRequired { get; set; } = "Urun zorunludur.";
        public static string OfferPriceRequired { get; set; } = "Fiyat zorunludur.";
        public static string OfferQuantityRequired { get; set; } = "Miktar zorunludur.";
        public static string OfferBuyerRequired { get; set; } = "Alici zorunludur.";
        public static string OfferSellerRequired { get; set; } = "Satici zorunludur.";
    }

    public static class DealErrors
    {
        public static string DealNotFound { get; set; } = "Anlasma bulunamadi.";
        public static string DealNumberRequired { get; set; } = "Anlasma numarasi zorunludur.";
        public static string DealProductRequired { get; set; } = "Urun zorunludur.";
        public static string DealSellerRequired { get; set; } = "Satici zorunludur.";
        public static string DealBuyerRequired { get; set; } = "Alici zorunludur.";
        public static string DealPriceRequired { get; set; } = "Fiyat zorunludur.";
    }

    public static class TransporterErrors
    {
        public static string TransporterNotFound { get; set; } = "Tasiyici bulunamadi.";
        public static string TransporterCompanyNameRequired { get; set; } = "Sirket adi zorunludur.";
        public static string TransporterEmailRequired { get; set; } = "E-posta zorunludur.";
        public static string TransporterUserRequired { get; set; } = "Kullanici zorunludur.";
        public static string TransporterNotPendingVerification { get; set; } = "Tasiyici dogrulama bekleyen durumda degil.";
        public static string TransporterAlreadySuspended { get; set; } = "Tasiyici zaten askiya alinmis.";
        public static string SuspensionReasonRequired { get; set; } = "Askiya alma nedeni zorunludur.";
    }

    public static class TransportRequestErrors
    {
        public static string TransportRequestNotFound { get; set; } = "Tasima talebi bulunamadi.";
        public static string TransportRequestProductRequired { get; set; } = "Urun zorunludur.";
        public static string TransportRequestSellerRequired { get; set; } = "Satici zorunludur.";
        public static string TransportRequestBuyerRequired { get; set; } = "Alici zorunludur.";
        public static string TransportRequestPickupLocationRequired { get; set; } = "Alinacak konum zorunludur.";
        public static string TransportRequestDeliveryLocationRequired { get; set; } = "Teslimat konumu zorunludur.";
    }

    public static class TransportOfferErrors
    {
        public static string TransportOfferNotFound { get; set; } = "Tasima teklifi bulunamadi.";
        public static string TransportOfferRequestRequired { get; set; } = "Tasima talebi zorunludur.";
        public static string TransportOfferTransporterRequired { get; set; } = "Tasiyici zorunludur.";
        public static string TransportOfferPriceRequired { get; set; } = "Fiyat zorunludur.";
    }

    public static class TransportTrackingErrors
    {
        public static string TransportTrackingNotFound { get; set; } = "Takip kaydi bulunamadi.";
        public static string TransportTrackingRequestRequired { get; set; } = "Tasima talebi zorunludur.";
    }

    public static class CurrencyErrors
    {
        public static string CurrencyNotFound { get; set; } = "Para birimi bulunamadi.";
        public static string CurrencyCodeRequired { get; set; } = "Para birimi kodu zorunludur.";
        public static string CurrencyNameRequired { get; set; } = "Para birimi adi zorunludur.";
        public static string CurrencySymbolRequired { get; set; } = "Para birimi sembolu zorunludur.";
    }

    public static class LanguageErrors
    {
        public static string LanguageNotFound { get; set; } = "Dil bulunamadi.";
        public static string LanguageCodeRequired { get; set; } = "Dil kodu zorunludur.";
        public static string LanguageNameRequired { get; set; } = "Dil adi zorunludur.";
    }

    public static class PaymentMethodErrors
    {
        public static string PaymentMethodNotFound { get; set; } = "Odeme yontemi bulunamadi.";
        public static string PaymentMethodNameRequired { get; set; } = "Odeme yontemi adi zorunludur.";
        public static string PaymentMethodCodeRequired { get; set; } = "Odeme yontemi kodu zorunludur.";
    }

    public static class ShippingCarrierErrors
    {
        public static string ShippingCarrierNotFound { get; set; } = "Kargo tasiyicisi bulunamadi.";
        public static string ShippingCarrierNameRequired { get; set; } = "Tasiyici adi zorunludur.";
        public static string ShippingCarrierCodeRequired { get; set; } = "Tasiyici kodu zorunludur.";
    }

    public static class FAQErrors
    {
        public static string FAQNotFound { get; set; } = "SSS bulunamadi.";
        public static string FAQQuestionRequired { get; set; } = "Soru zorunludur.";
        public static string FAQAnswerRequired { get; set; } = "Cevap zorunludur.";
    }

    public static class BannerErrors
    {
        public static string BannerNotFound { get; set; } = "Banner bulunamadi.";
        public static string BannerTitleRequired { get; set; } = "Baslik zorunludur.";
        public static string BannerImageUrlRequired { get; set; } = "Gorsel URL zorunludur.";
        public static string BannerDateRequired { get; set; } = "Tarih zorunludur.";
        public static string BannerStartDateRequired { get; set; } = "Baslangic tarihi zorunludur.";
        public static string BannerEndDateRequired { get; set; } = "Bitis tarihi zorunludur.";
    }

    public static class TaxRateErrors
    {
        public static string TaxRateNotFound { get; set; } = "Vergi orani bulunamadi.";
        public static string TaxRateCountryCodeRequired { get; set; } = "Ulke kodu zorunludur.";
        public static string TaxRateNameRequired { get; set; } = "Vergi adi zorunludur.";
        public static string TaxRateAmountRequired { get; set; } = "Oran zorunludur.";
    }

    public static class ShippingZoneErrors
    {
        public static string ShippingZoneNotFound { get; set; } = "Kargo bolgesi bulunamadi.";
        public static string ShippingZoneNameRequired { get; set; } = "Bolge adi zorunludur.";
    }

    public static class ShippingRateErrors
    {
        public static string ShippingRateNotFound { get; set; } = "Kargo ucreti bulunamadi.";
        public static string ShippingRateZoneRequired { get; set; } = "Kargo bolgesi zorunludur.";
        public static string ShippingRateCostRequired { get; set; } = "Kargo ucreti zorunludur.";
    }

    public static class AuthorizationErrors
    {
        public static string InsufficientPermission { get; set; } = "Bu islem icin yetkiniz yok.";
        public static string AdminRequired { get; set; } = "Bu islem icin Admin yetkisi gereklidir.";
        public static string ModeratorRequired { get; set; } = "Bu islem icin Moderator yetkisi gereklidir.";
        public static string StaffRequired { get; set; } = "Bu islem icin personel yetkisi gereklidir.";
        public static string SellerRequired { get; set; } = "Bu islem icin Satici yetkisi gereklidir.";
        public static string TransporterRequired { get; set; } = "Bu islem icin Nakliyeci yetkisi gereklidir.";
        public static string VeterinarianRequired { get; set; } = "Bu islem icin Veteriner yetkisi gereklidir.";
        public static string OwnershipRequired { get; set; } = "Bu kayit uzerinde islem yapma yetkiniz yok.";
        public static string ResourceNotAccessible { get; set; } = "Bu kaynaga erisim yetkiniz yok.";
    }
}
