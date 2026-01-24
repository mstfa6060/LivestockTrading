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
}
