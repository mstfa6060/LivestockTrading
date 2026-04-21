namespace Gateways.ApiGateway.Api;

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
					config.AddJsonFile("ocelot.json");
				})
			.ConfigureWebHostDefaults(webBuilder =>
			{
				webBuilder.UseStartup<Startup>();
				webBuilder.ConfigureKestrel((context, serverOptions) =>
				{
					serverOptions.Limits.MaxRequestBodySize = 500_000_000;
				});
			});
}