namespace Common.Services.Environment;

public class BaseUrls
{
    private static string HttpProtocol = "https";
    private static string BaseDomain = "toprak.tkholding.com.tr";
    private static string TestApiPrefix = "api-test";
    private static string ProductionApiPrefix = "api";
    private static string TestUiPrefix = "test";

    public static string GetUiUrl(CustomEnvironments environment)
    {
        switch (environment)
        {
            case CustomEnvironments.Production:
                return $"{HttpProtocol}://{BaseDomain}";
            case CustomEnvironments.Staging:
                return $"{HttpProtocol}://{TestUiPrefix}.{BaseDomain}";
            case CustomEnvironments.Development:
                return $"{HttpProtocol}://{TestUiPrefix}.{BaseDomain}";
            default:
                return "not.handled.environment.enum";
        }
    }

    public static string GetApiUrl(CustomEnvironments environment)
    {
        switch (environment)
        {
            case CustomEnvironments.Production:
                return $"{HttpProtocol}://{ProductionApiPrefix}.{BaseDomain}";
            case CustomEnvironments.Staging:
                return $"{HttpProtocol}://{TestApiPrefix}.{BaseDomain}";
            case CustomEnvironments.Development:
                return $"http://localhost:5000";
            default:
                return "not.handled.environment.enum";
        }
    }
}