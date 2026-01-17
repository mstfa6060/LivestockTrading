namespace Common.Services.Environment;

public class ExternalAuthConfiguration
{
    public GoogleClient Google { get; set; }
    public AppleClient Apple { get; set; }
    public class GoogleClient
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
    }
    public class AppleClient
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
    }
}