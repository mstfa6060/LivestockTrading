namespace Files.Features.Services;

public sealed class MinioSettings
{
    public string Endpoint { get; set; } = "localhost:9000";
    public string AccessKey { get; set; } = "minioadmin";
    public string SecretKey { get; set; } = "minioadmin";
    public bool UseSSL { get; set; }
    public string BucketName { get; set; } = "livestocktrading";
}
