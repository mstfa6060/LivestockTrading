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
        public static string SlugAlreadyExists { get; set; } = "Bu slug zaten kullaniliyor.";
        public static string NameAlreadyExists { get; set; } = "Bu isimde bir kategori zaten mevcut.";
        public static string CategoryHasChildren { get; set; } = "Alt kategorileri olan bir kategori silinemez.";
        public static string NameRequired { get; set; } = "Kategori adi zorunludur.";
        public static string SlugRequired { get; set; } = "Kategori slug zorunludur.";
    }

    public static class BrandErrors
    {
        public static string BrandNotFound { get; set; } = "Marka bulunamadi.";
        public static string BrandNotActive { get; set; } = "Marka aktif degil.";
        public static string SlugAlreadyExists { get; set; } = "Bu slug zaten kullaniliyor.";
        public static string NameRequired { get; set; } = "Marka adi zorunludur.";
        public static string SlugRequired { get; set; } = "Marka slug zorunludur.";
    }

    public static class ProductErrors
    {
        public static string ProductNotFound { get; set; } = "Urun bulunamadi.";
        public static string ProductNotActive { get; set; } = "Urun aktif degil.";
        public static string SlugAlreadyExists { get; set; } = "Bu slug zaten kullaniliyor.";
        public static string TitleRequired { get; set; } = "Urun basligi zorunludur.";
        public static string SlugRequired { get; set; } = "Urun slug zorunludur.";
        public static string CategoryRequired { get; set; } = "Kategori zorunludur.";
        public static string SellerRequired { get; set; } = "Satici zorunludur.";
        public static string LocationRequired { get; set; } = "Konum zorunludur.";
        public static string PriceRequired { get; set; } = "Fiyat zorunludur.";
    }

    public static class LocationErrors
    {
        public static string LocationNotFound { get; set; } = "Konum bulunamadi.";
        public static string NameRequired { get; set; } = "Konum adi zorunludur.";
    }

    public static class SellerErrors
    {
        public static string SellerNotFound { get; set; } = "Satici bulunamadi.";
        public static string BusinessNameRequired { get; set; } = "Isletme adi zorunludur.";
        public static string UserIdRequired { get; set; } = "Kullanici ID zorunludur.";
    }

    public static class FarmErrors
    {
        public static string FarmNotFound { get; set; } = "Ciftlik bulunamadi.";
        public static string NameRequired { get; set; } = "Ciftlik adi zorunludur.";
        public static string SellerRequired { get; set; } = "Satici zorunludur.";
        public static string LocationRequired { get; set; } = "Konum zorunludur.";
    }

    public static class ProductVariantErrors
    {
        public static string VariantNotFound { get; set; } = "Urun varyanti bulunamadi.";
        public static string NameRequired { get; set; } = "Varyant adi zorunludur.";
        public static string ProductRequired { get; set; } = "Urun zorunludur.";
    }

    public static class ProductPriceErrors
    {
        public static string PriceNotFound { get; set; } = "Fiyat bilgisi bulunamadi.";
        public static string CurrencyCodeRequired { get; set; } = "Para birimi kodu zorunludur.";
        public static string PriceRequired { get; set; } = "Fiyat zorunludur.";
        public static string ProductRequired { get; set; } = "Urun zorunludur.";
    }

    public static class ProductImageErrors
    {
        public static string ImageNotFound { get; set; } = "Gorsel bulunamadi.";
        public static string ImageUrlRequired { get; set; } = "Gorsel URL zorunludur.";
        public static string ProductRequired { get; set; } = "Urun zorunludur.";
    }

    public static class ProductVideoErrors
    {
        public static string VideoNotFound { get; set; } = "Video bulunamadi.";
        public static string VideoUrlRequired { get; set; } = "Video URL zorunludur.";
        public static string ProductRequired { get; set; } = "Urun zorunludur.";
    }

    public static class ProductDocumentErrors
    {
        public static string DocumentNotFound { get; set; } = "Dokuman bulunamadi.";
        public static string DocumentUrlRequired { get; set; } = "Dokuman URL zorunludur.";
        public static string FileNameRequired { get; set; } = "Dosya adi zorunludur.";
        public static string ProductRequired { get; set; } = "Urun zorunludur.";
    }

    public static class AnimalInfoErrors
    {
        public static string AnimalInfoNotFound { get; set; } = "Hayvan bilgisi bulunamadi.";
        public static string BreedNameRequired { get; set; } = "Irk adi zorunludur.";
        public static string ProductRequired { get; set; } = "Urun zorunludur.";
    }

    public static class HealthRecordErrors
    {
        public static string HealthRecordNotFound { get; set; } = "Saglik kaydi bulunamadi.";
        public static string RecordTypeRequired { get; set; } = "Kayit tipi zorunludur.";
        public static string RecordDateRequired { get; set; } = "Kayit tarihi zorunludur.";
        public static string AnimalInfoRequired { get; set; } = "Hayvan bilgisi zorunludur.";
    }

    public static class VaccinationErrors
    {
        public static string VaccinationNotFound { get; set; } = "Asilanma kaydi bulunamadi.";
        public static string VaccineNameRequired { get; set; } = "Asi adi zorunludur.";
        public static string VaccinationDateRequired { get; set; } = "Asilanma tarihi zorunludur.";
        public static string AnimalInfoRequired { get; set; } = "Hayvan bilgisi zorunludur.";
    }

    public static class ChemicalInfoErrors
    {
        public static string ChemicalInfoNotFound { get; set; } = "Kimyasal bilgisi bulunamadi.";
        public static string ProductRequired { get; set; } = "Urun zorunludur.";
    }

    public static class MachineryInfoErrors
    {
        public static string MachineryInfoNotFound { get; set; } = "Makine bilgisi bulunamadi.";
        public static string ProductRequired { get; set; } = "Urun zorunludur.";
    }

    public static class SeedInfoErrors
    {
        public static string SeedInfoNotFound { get; set; } = "Tohum bilgisi bulunamadi.";
        public static string ProductRequired { get; set; } = "Urun zorunludur.";
    }

    public static class FeedInfoErrors
    {
        public static string FeedInfoNotFound { get; set; } = "Yem bilgisi bulunamadi.";
        public static string ProductRequired { get; set; } = "Urun zorunludur.";
    }

    public static class VeterinaryInfoErrors
    {
        public static string VeterinaryInfoNotFound { get; set; } = "Veteriner urun bilgisi bulunamadi.";
        public static string ProductRequired { get; set; } = "Urun zorunludur.";
    }

    public static class ProductReviewErrors
    {
        public static string ReviewNotFound { get; set; } = "Degerlendirme bulunamadi.";
        public static string ProductRequired { get; set; } = "Urun zorunludur.";
        public static string UserRequired { get; set; } = "Kullanici zorunludur.";
        public static string RatingRequired { get; set; } = "Puan zorunludur.";
    }

    public static class SellerReviewErrors
    {
        public static string ReviewNotFound { get; set; } = "Satici degerlendirmesi bulunamadi.";
        public static string SellerRequired { get; set; } = "Satici zorunludur.";
        public static string UserRequired { get; set; } = "Kullanici zorunludur.";
        public static string RatingRequired { get; set; } = "Puan zorunludur.";
    }

    public static class TransporterReviewErrors
    {
        public static string ReviewNotFound { get; set; } = "Tasiyici degerlendirmesi bulunamadi.";
        public static string TransporterRequired { get; set; } = "Tasiyici zorunludur.";
        public static string UserRequired { get; set; } = "Kullanici zorunludur.";
        public static string RatingRequired { get; set; } = "Puan zorunludur.";
    }

    public static class FavoriteProductErrors
    {
        public static string FavoriteNotFound { get; set; } = "Favori bulunamadi.";
        public static string ProductRequired { get; set; } = "Urun zorunludur.";
        public static string UserRequired { get; set; } = "Kullanici zorunludur.";
    }

    public static class NotificationErrors
    {
        public static string NotificationNotFound { get; set; } = "Bildirim bulunamadi.";
        public static string TitleRequired { get; set; } = "Baslik zorunludur.";
        public static string MessageRequired { get; set; } = "Mesaj zorunludur.";
        public static string UserRequired { get; set; } = "Kullanici zorunludur.";
    }

    public static class SearchHistoryErrors
    {
        public static string SearchHistoryNotFound { get; set; } = "Arama gecmisi bulunamadi.";
        public static string SearchQueryRequired { get; set; } = "Arama sorgusu zorunludur.";
        public static string UserRequired { get; set; } = "Kullanici zorunludur.";
    }

    public static class ProductViewHistoryErrors
    {
        public static string ViewHistoryNotFound { get; set; } = "Goruntuleme gecmisi bulunamadi.";
        public static string ProductRequired { get; set; } = "Urun zorunludur.";
        public static string UserRequired { get; set; } = "Kullanici zorunludur.";
    }

    public static class UserPreferencesErrors
    {
        public static string PreferencesNotFound { get; set; } = "Kullanici tercihleri bulunamadi.";
        public static string UserRequired { get; set; } = "Kullanici zorunludur.";
    }

    public static class ConversationErrors
    {
        public static string ConversationNotFound { get; set; } = "Konusma bulunamadi.";
        public static string ParticipantsRequired { get; set; } = "Katilimcilar zorunludur.";
    }

    public static class MessageErrors
    {
        public static string MessageNotFound { get; set; } = "Mesaj bulunamadi.";
        public static string ContentRequired { get; set; } = "Icerik zorunludur.";
        public static string ConversationRequired { get; set; } = "Konusma zorunludur.";
        public static string SenderRequired { get; set; } = "Gonderen zorunludur.";
        public static string RecipientRequired { get; set; } = "Alici zorunludur.";
    }

    public static class OfferErrors
    {
        public static string OfferNotFound { get; set; } = "Teklif bulunamadi.";
        public static string ProductRequired { get; set; } = "Urun zorunludur.";
        public static string PriceRequired { get; set; } = "Fiyat zorunludur.";
        public static string QuantityRequired { get; set; } = "Miktar zorunludur.";
        public static string BuyerRequired { get; set; } = "Alici zorunludur.";
        public static string SellerRequired { get; set; } = "Satici zorunludur.";
    }

    public static class DealErrors
    {
        public static string DealNotFound { get; set; } = "Anlasma bulunamadi.";
        public static string DealNumberRequired { get; set; } = "Anlasma numarasi zorunludur.";
        public static string ProductRequired { get; set; } = "Urun zorunludur.";
        public static string SellerRequired { get; set; } = "Satici zorunludur.";
        public static string BuyerRequired { get; set; } = "Alici zorunludur.";
        public static string PriceRequired { get; set; } = "Fiyat zorunludur.";
    }

    public static class TransporterErrors
    {
        public static string TransporterNotFound { get; set; } = "Tasiyici bulunamadi.";
        public static string CompanyNameRequired { get; set; } = "Sirket adi zorunludur.";
        public static string EmailRequired { get; set; } = "E-posta zorunludur.";
        public static string UserRequired { get; set; } = "Kullanici zorunludur.";
    }

    public static class TransportRequestErrors
    {
        public static string RequestNotFound { get; set; } = "Tasima talebi bulunamadi.";
        public static string ProductRequired { get; set; } = "Urun zorunludur.";
        public static string SellerRequired { get; set; } = "Satici zorunludur.";
        public static string BuyerRequired { get; set; } = "Alici zorunludur.";
        public static string PickupLocationRequired { get; set; } = "Alinacak konum zorunludur.";
        public static string DeliveryLocationRequired { get; set; } = "Teslimat konumu zorunludur.";
    }

    public static class TransportOfferErrors
    {
        public static string OfferNotFound { get; set; } = "Tasima teklifi bulunamadi.";
        public static string TransportRequestRequired { get; set; } = "Tasima talebi zorunludur.";
        public static string TransporterRequired { get; set; } = "Tasiyici zorunludur.";
        public static string PriceRequired { get; set; } = "Fiyat zorunludur.";
    }

    public static class TransportTrackingErrors
    {
        public static string TrackingNotFound { get; set; } = "Takip kaydi bulunamadi.";
        public static string TransportRequestRequired { get; set; } = "Tasima talebi zorunludur.";
    }

    public static class CurrencyErrors
    {
        public static string CurrencyNotFound { get; set; } = "Para birimi bulunamadi.";
        public static string CodeRequired { get; set; } = "Para birimi kodu zorunludur.";
        public static string NameRequired { get; set; } = "Para birimi adi zorunludur.";
        public static string SymbolRequired { get; set; } = "Para birimi sembolu zorunludur.";
    }

    public static class LanguageErrors
    {
        public static string LanguageNotFound { get; set; } = "Dil bulunamadi.";
        public static string CodeRequired { get; set; } = "Dil kodu zorunludur.";
        public static string NameRequired { get; set; } = "Dil adi zorunludur.";
    }

    public static class PaymentMethodErrors
    {
        public static string PaymentMethodNotFound { get; set; } = "Odeme yontemi bulunamadi.";
        public static string NameRequired { get; set; } = "Odeme yontemi adi zorunludur.";
        public static string CodeRequired { get; set; } = "Odeme yontemi kodu zorunludur.";
    }

    public static class ShippingCarrierErrors
    {
        public static string CarrierNotFound { get; set; } = "Kargo tasiyicisi bulunamadi.";
        public static string NameRequired { get; set; } = "Tasiyici adi zorunludur.";
        public static string CodeRequired { get; set; } = "Tasiyici kodu zorunludur.";
    }

    public static class FAQErrors
    {
        public static string FAQNotFound { get; set; } = "SSS bulunamadi.";
        public static string QuestionRequired { get; set; } = "Soru zorunludur.";
        public static string AnswerRequired { get; set; } = "Cevap zorunludur.";
    }

    public static class BannerErrors
    {
        public static string BannerNotFound { get; set; } = "Banner bulunamadi.";
        public static string TitleRequired { get; set; } = "Baslik zorunludur.";
        public static string ImageUrlRequired { get; set; } = "Gorsel URL zorunludur.";
        public static string DateRequired { get; set; } = "Tarih zorunludur.";
        public static string StartDateRequired { get; set; } = "Baslangic tarihi zorunludur.";
        public static string EndDateRequired { get; set; } = "Bitis tarihi zorunludur.";
    }

    public static class TaxRateErrors
    {
        public static string TaxRateNotFound { get; set; } = "Vergi orani bulunamadi.";
        public static string CountryCodeRequired { get; set; } = "Ulke kodu zorunludur.";
        public static string TaxNameRequired { get; set; } = "Vergi adi zorunludur.";
        public static string RateRequired { get; set; } = "Oran zorunludur.";
    }

    public static class ShippingZoneErrors
    {
        public static string ZoneNotFound { get; set; } = "Kargo bolgesi bulunamadi.";
        public static string NameRequired { get; set; } = "Bolge adi zorunludur.";
    }

    public static class ShippingRateErrors
    {
        public static string RateNotFound { get; set; } = "Kargo ucreti bulunamadi.";
        public static string ShippingZoneRequired { get; set; } = "Kargo bolgesi zorunludur.";
        public static string ShippingCostRequired { get; set; } = "Kargo ucreti zorunludur.";
    }
}
