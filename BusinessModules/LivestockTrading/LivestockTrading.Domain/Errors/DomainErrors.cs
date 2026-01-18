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
}
