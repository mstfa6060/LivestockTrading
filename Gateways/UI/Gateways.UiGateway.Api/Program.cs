namespace Gateways.UiGateway.Api;

public class Program
{
    public static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
                // Ocelot Configuration
                .ConfigureAppConfiguration((host, config) =>
                {
                    config.AddJsonFile("configuration.json");
                })
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            });
}