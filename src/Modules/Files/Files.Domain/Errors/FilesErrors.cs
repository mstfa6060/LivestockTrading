namespace Files.Domain.Errors;

public static class FilesErrors
{
    public static class Buckets
    {
        public const string NotFound = "Bucket bulunamadı.";
        public const string NotOwned = "Bu bucket'a erişim yetkiniz yok.";
    }

    public static class Files
    {
        public const string NotFound = "Dosya bulunamadı.";
        public const string TooLarge = "Dosya boyutu çok büyük (max 75 MB).";
        public const string InvalidType = "Desteklenmeyen dosya türü.";
        public const string SingleBucketFull = "Bu bucket yalnızca bir dosya destekler.";
        public const string OrderInvalid = "Sıralama listesi geçersiz.";
    }
}
