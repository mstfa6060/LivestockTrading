namespace Files.Domain.Errors;

// Symbolic codes — see IamErrors.cs for rationale.
public static class FilesErrors
{
    public static class Buckets
    {
        public const string NotFound = "FILES_BUCKET_NOT_FOUND";
        public const string NotOwned = "FILES_BUCKET_NOT_OWNED";
    }

    public static class Files
    {
        public const string NotFound = "FILES_FILE_NOT_FOUND";
        public const string TooLarge = "FILES_FILE_TOO_LARGE";
        public const string InvalidType = "FILES_FILE_INVALID_TYPE";
        public const string SingleBucketFull = "FILES_BUCKET_SINGLE_FULL";
        public const string OrderInvalid = "FILES_ORDER_INVALID";
    }
}
