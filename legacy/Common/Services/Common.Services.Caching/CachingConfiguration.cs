namespace Common.Services.Caching;

public class CachingConfiguration
{
    public RedisConfiguration Redis { get; set; } = new();
    public MemoryConfiguration Memory { get; set; } = new();
}

public class RedisConfiguration
{
    public string ConnectionString { get; set; } = "localhost:6379";
    public string InstanceName { get; set; } = "maden:";
    public int DefaultExpirationMinutes { get; set; } = 60;
}

public class MemoryConfiguration
{
    public int SizeLimitMB { get; set; } = 1024;
    public double CompactionPercentage { get; set; } = 0.25;
}
